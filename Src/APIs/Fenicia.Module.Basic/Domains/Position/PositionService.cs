using Fenicia.Common;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Position.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Position;

public class PositionService
{
    private readonly IPositionRepository _positionRepository;

    public PositionService()
        : this(null!)
    {
    }

    public PositionService(IPositionRepository positionRepository)
    {
        _positionRepository = positionRepository;
    }

    public virtual async Task<Pagination<List<GetAllPositionResponse>>> GetAllAsync(GetAllPositionQuery query, CancellationToken ct)
    {
        var total = await _positionRepository.CountAsync(ct);

        var positions = await _positionRepository.Query()
            .Select(p => p.MapToGetAllPositionResponse())
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(ct);

        return new Pagination<List<GetAllPositionResponse>>(positions, total, query.Page, query.PerPage);
    }

    public virtual async Task<GetPositionByIdResponse?> GetByIdAsync(GetPositionByIdQuery query, CancellationToken ct)
    {
        var position = await _positionRepository.GetByIdAsync(query.Id, ct);

        return position is null ? null : position.MapToGetPositionByIdResponse();
    }

    public virtual async Task<AddPositionResponse> AddAsync(AddPositionCommand command, Guid companyId, CancellationToken ct)
    {
        var position = new PositionModel
        {
            Id = command.Id,
            Name = command.Name,
            CompanyId = companyId
        };

        await _positionRepository.InsertAsync(position, ct);

        return position.MapToAddPositionResponse();
    }

    public virtual async Task<UpdatePositionResponse?> UpdateAsync(UpdatePositionCommand command, Guid companyId, CancellationToken ct)
    {
        var position = await _positionRepository.GetByIdAsync(command.Id, ct);

        if (position is null)
        {
            return null;
        }

        position.Name = command.Name;
        position.CompanyId = companyId;

        await _positionRepository.UpdateAsync(command.Id, position, ct);

        return position.MapToUpdatePositionResponse();
    }

    public virtual async Task DeleteAsync(DeletePositionCommand command, Guid companyId, CancellationToken ct)
    {
        await _positionRepository.DeleteAsync(command.Id, ct);
    }
}
