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

    public virtual async Task<Pagination<List<GetAllPositionResponse>>> GetAllAsync(GetAllPositionQuery query, CancellationToken cancellationToken = default)
    {
        var total = await _positionRepository.CountAsync(cancellationToken);

        var positions = await _positionRepository.Query()
            .Select(p => p.MapToGetAllPositionResponse())
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(cancellationToken);

        return new Pagination<List<GetAllPositionResponse>>(positions, total, query.Page, query.PerPage);
    }

    public virtual async Task<GetPositionByIdResponse?> GetByIdAsync(GetPositionByIdQuery query, CancellationToken cancellationToken = default)
    {
        var position = await _positionRepository.GetByIdAsync(query.Id, cancellationToken);

        return position is null ? null : position.MapToGetPositionByIdResponse();
    }

    public virtual async Task<AddPositionResponse> AddAsync(AddPositionCommand command, Guid companyId, CancellationToken cancellationToken = default)
    {
        var position = new PositionModel
        {
            Id = command.Id,
            Name = command.Name,
            CompanyId = companyId
        };

        await _positionRepository.InsertAsync(position, cancellationToken);

        return position.MapToAddPositionResponse();
    }

    public virtual async Task<UpdatePositionResponse?> UpdateAsync(UpdatePositionCommand command, Guid companyId, CancellationToken cancellationToken = default)
    {
        var position = await _positionRepository.GetByIdAsync(command.Id, cancellationToken);

        if (position is null)
        {
            return null;
        }

        position.Name = command.Name;
        position.CompanyId = companyId;

        await _positionRepository.UpdateAsync(command.Id, position, cancellationToken);

        return position.MapToUpdatePositionResponse();
    }

    public virtual async Task DeleteAsync(DeletePositionCommand command, Guid companyId, CancellationToken cancellationToken = default)
    {
        await _positionRepository.DeleteAsync(command.Id, cancellationToken);
    }
}
