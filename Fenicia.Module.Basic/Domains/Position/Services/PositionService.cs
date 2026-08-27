using Fenicia.Common;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Position.DTOs.Commands;
using Fenicia.Module.Basic.Domains.Position.DTOs.Queries;
using Fenicia.Module.Basic.Domains.Position.DTOs.Responses;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Position;

public class PositionService(DefaultContext db)
{
    public async Task<Pagination<List<GetAllPositionResponse>>> GetAllAsync(GetAllPositionQuery query, CancellationToken ct)
    {
        var total = await db.BasicPositions.CountAsync(ct);

        var positions = await db.BasicPositions.Select(p => new GetAllPositionResponse(p.Id, p.Name)).Skip((query.Page - 1) * query.PerPage).Take(query.PerPage).ToListAsync(ct);

        return new Pagination<List<GetAllPositionResponse>>(positions, total, query.Page, query.PerPage);
    }

    public async Task<GetPositionByIdResponse?> GetByIdAsync(GetPositionByIdQuery query, CancellationToken ct)
    {
        var position = await db.BasicPositions.FirstOrDefaultAsync(p => p.Id == query.Id, ct);

        return position is null ? null : new GetPositionByIdResponse(position.Id, position.Name);
    }

    public async Task<AddPositionResponse> AddAsync(AddPositionCommand command, CancellationToken ct)
    {
        var position = new PositionModel
        {
            Id = command.Id,
            Name = command.Name
        };

        db.BasicPositions.Add(position);

        await db.SaveChangesAsync(ct);

        return new AddPositionResponse(position.Id, position.Name);
    }

    public async Task<UpdatePositionResponse?> UpdateAsync(UpdatePositionCommand command, CancellationToken ct)
    {
        var position = await db.BasicPositions.FirstOrDefaultAsync(p => p.Id == command.Id, ct);

        if (position is null)
        {
            return null;
        }

        position.Name = command.Name;

        db.BasicPositions.Update(position);

        await db.SaveChangesAsync(ct);

        return new UpdatePositionResponse(position.Id, position.Name);
    }

    public async Task DeleteAsync(DeletePositionCommand command, CancellationToken ct)
    {
        var position = await db.BasicPositions.FirstOrDefaultAsync(p => p.Id == command.Id, ct);

        if (position is null)
        {
            return;
        }

        position.Deleted = DateTime.Now;

        db.BasicPositions.Update(position);

        await db.SaveChangesAsync(ct);
    }
}
