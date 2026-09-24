using Fenicia.Common;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.DTOs.Basic.Position;
using Fenicia.Module.Basic.Domains.Position.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Position;

/// <summary>
/// Service implementation for managing positions.
/// </summary>
/// <param name="repository">The position repository.</param>
public sealed class PositionService(IPositionRepository repository) : IPositionService
{
    /// <inheritdoc />
    public async Task<Pagination<List<PositionResponse>>> GetAllAsync(
        PositionQuery query,
        CancellationToken cancellationToken = default)
    {
        var baseQuery = repository.Query();

        var filteredQuery = baseQuery.ApplySearch(query.Query, "Name").ApplyFilters(query.Filters).ApplySort(query.Sort);

        var total = await filteredQuery.CountAsync(cancellationToken);

        var positions = await filteredQuery
            .Select(p => new PositionResponse(p.Id, p.Name))
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(cancellationToken);

        return new Pagination<List<PositionResponse>>(positions, total, query);
    }

    /// <inheritdoc />
    public async Task<PositionResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var position = await repository.GetByIdAsync(id, cancellationToken);

        return position is null ? null : PositionMapper.MapToResponse(position);
    }

    /// <inheritdoc />
    public async Task<PositionResponse> AddAsync(
        PositionRequest command,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        var position = new PositionModel
        {
            Name = command.Name,
            CompanyId = companyId
        };

        await repository.InsertAsync(position, cancellationToken);

        return PositionMapper.MapToResponse(position);
    }

    /// <inheritdoc />
    public async Task<PositionResponse?> UpdateAsync(
        Guid id,
        PositionRequest command,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        var position = await repository.GetByIdAsync(id, cancellationToken);

        if (position is null)
        {
            return null;
        }

        position.Name = command.Name;
        position.CompanyId = companyId;

        await repository.UpdateAsync(id, position, cancellationToken);

        return PositionMapper.MapToResponse(position);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(
        Guid id,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        await repository.DeleteAsync(id, cancellationToken);
    }
}