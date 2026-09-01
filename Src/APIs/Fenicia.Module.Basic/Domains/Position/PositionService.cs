using Fenicia.Common;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Position.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Position;

public class PositionService(IPositionRepository positionRepository)
{
    public PositionService()
        : this(null!)
    {
    }

    public virtual async Task<Pagination<List<GetAllPositionResponse>>> GetAllAsync(GetAllPositionQuery query, CancellationToken cancellationToken = default)
    {
        var baseQuery = positionRepository.Query();

        var filters = AdvancedQueryParser.Parse(query.Query);
        var filteredQuery = baseQuery.ApplyAdvancedQuery(filters, query.Sort);

        var total = await filteredQuery.CountAsync(cancellationToken);

        var positions = await filteredQuery
            .Select(p => p.MapToGetAllPositionResponse())
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(cancellationToken);

        return new Pagination<List<GetAllPositionResponse>>(positions, total, query.Page, query.PerPage);
    }

    public virtual async Task<GetPositionByIdResponse?> GetByIdAsync(GetPositionByIdQuery query, CancellationToken cancellationToken = default)
    {
        var position = await positionRepository.GetByIdAsync(query.Id, cancellationToken);

        return position?.MapToGetPositionByIdResponse();
    }

    public virtual async Task<AddPositionResponse> AddAsync(AddPositionCommand command, Guid companyId, CancellationToken cancellationToken = default)
    {
        var position = new PositionModel
        {
            Id = command.Id,
            Name = command.Name,
            CompanyId = companyId
        };

        await positionRepository.InsertAsync(position, cancellationToken);

        return position.MapToAddPositionResponse();
    }

    public virtual async Task<UpdatePositionResponse?> UpdateAsync(UpdatePositionCommand command, Guid companyId, CancellationToken cancellationToken = default)
    {
        var position = await positionRepository.GetByIdAsync(command.Id, cancellationToken);

        if (position is null)
        {
            return null;
        }

        position.Name = command.Name;
        position.CompanyId = companyId;

        await positionRepository.UpdateAsync(command.Id, position, cancellationToken);

        return position.MapToUpdatePositionResponse();
    }

    public virtual async Task DeleteAsync(DeletePositionCommand command, Guid companyId, CancellationToken cancellationToken = default)
    {
        await positionRepository.DeleteAsync(command.Id, cancellationToken);
    }
}
