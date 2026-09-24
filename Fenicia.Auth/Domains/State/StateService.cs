using Fenicia.Auth.Domains.State.Interfaces;
using Fenicia.Common.DTOs.Auth.State;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.State;

/// <summary>
/// Service responsible for managing state-related operations.
/// </summary>
/// <param name="repository">The repository for managing state data.</param>
public sealed class StateService(IStateRepository repository) : IStateService
{
    /// <summary>
    /// Retrieves all states with pagination and filtering.
    /// </summary>
    /// <param name="query">The query parameters for filtering and pagination.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of state responses.</returns>
    public async Task<List<StateResponse>> GetAllAsync(
        StateRequest query,
        CancellationToken cancellationToken = default)
    {
        var baseQuery = repository.Query();

        var states = await baseQuery
            .OrderBy(s => s.Uf)
            .ToListAsync(cancellationToken);

        return [.. states.Select(s => new StateResponse(s.Id, s.Name, s.Uf))];
    }
}