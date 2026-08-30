using Fenicia.Common;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Position.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Position;

public class PositionService(PositionRepository positionRepository)
{
    public PositionService()
        : this(null!)
    {
    }

    public async Task<Pagination<List<GetAllPositionResponse>>> GetAllAsync(GetAllPositionQuery query, CancellationToken ct)
    {
        var total = await positionRepository.CountAsync(ct);

        var positions = await positionRepository.Query()
            .Select(p => new GetAllPositionResponse(p.Id, p.Name))
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(ct);

        return new Pagination<List<GetAllPositionResponse>>(positions, total, query.Page, query.PerPage);
    }

    public async Task<GetPositionByIdResponse?> GetByIdAsync(GetPositionByIdQuery query, CancellationToken ct)
    {
        var position = await positionRepository.GetByIdAsync(query.Id, ct);

        return position is null ? null : new GetPositionByIdResponse(position.Id, position.Name);
    }

    public async Task<AddPositionResponse> AddAsync(AddPositionCommand command, Guid companyId, CancellationToken ct)
    {
        var position = new PositionModel
        {
            Id = command.Id,
            Name = command.Name,
            CompanyId = companyId
        };

        await positionRepository.InsertAsync(position, ct);

        return new AddPositionResponse(position.Id, position.Name);
    }

    public async Task<UpdatePositionResponse?> UpdateAsync(UpdatePositionCommand command, Guid companyId, CancellationToken ct)
    {
        var position = await positionRepository.GetByIdAsync(command.Id, ct);

        if (position is null)
        {
            return null;
        }

        position.Name = command.Name;
        position.CompanyId = companyId;

        await positionRepository.UpdateAsync(command.Id, position, ct);

        return new UpdatePositionResponse(position.Id, position.Name);
    }

    public async Task DeleteAsync(DeletePositionCommand command, Guid companyId, CancellationToken ct)
    {
        await positionRepository.DeleteAsync(command.Id, ct);
    }
}
