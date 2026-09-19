using Fenicia.Common;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.DTOs.Basic.Position;
using Fenicia.Module.Basic.Domains.Position.Interfaces;

namespace Fenicia.Module.Basic.Domains.Position;

public sealed class PositionService(IPositionRepository positionRepository, PositionMapper positionMapper) : IPositionService
{
    public PositionService()
        : this(null!, null!)
    {
    }

    public async Task<Pagination<List<GetAllPositionResponse>>> GetAllAsync(
        GetAllPositionQuery query,
        CancellationToken cancellationToken = default)
    {
        var baseQuery = positionRepository.Query();

        var filteredQuery = baseQuery.ApplySearch(query.Query, "Name").ApplyFilters(query.Filters).ApplySort(query.Sort);

        var total = await filteredQuery.CountAsync(cancellationToken);

        var positions = await filteredQuery
            .Select(p => positionMapper.MapToGetAllPositionResponse(p))
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(cancellationToken);

        return new Pagination<List<GetAllPositionResponse>>(positions, total, query.Page, query.PerPage);
    }

    public async Task<GetPositionByIdResponse?> GetByIdAsync(
        GetPositionByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var position = await positionRepository.GetByIdAsync(query.Id, cancellationToken);

        return position is null ? null : positionMapper.MapToGetPositionByIdResponse(position);
    }

    public async Task<AddPositionResponse> AddAsync(
        AddPositionCommand command,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        var position = new PositionModel
        {
            Name = command.Name,
            CompanyId = companyId
        };

        await positionRepository.InsertAsync(position, cancellationToken);

        return positionMapper.MapToAddPositionResponse(position);
    }

    public async Task<UpdatePositionResponse?> UpdateAsync(
        UpdatePositionCommand command,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        var position = await positionRepository.GetByIdAsync(command.Id, cancellationToken);

        if (position is null)
        {
            return null;
        }

        position.Name = command.Name;
        position.CompanyId = companyId;

        await positionRepository.UpdateAsync(command.Id, position, cancellationToken);

        return positionMapper.MapToUpdatePositionResponse(position);
    }

    public async Task DeleteAsync(
        DeletePositionCommand command,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        await positionRepository.DeleteAsync(command.Id, cancellationToken);
    }
}
