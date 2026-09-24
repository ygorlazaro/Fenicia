using Fenicia.Common.DTOs.Auth.State;

namespace Fenicia.Auth.Domains.State.Interfaces;

/// <summary>
/// Defines the contract for a service that manages state-related operations.
/// </summary>
public interface IStateService
{
    /// <summary>
    /// Retrieves all states with pagination and filtering.
    /// </summary>
    /// <param name="query">The query parameters for filtering and pagination.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of state responses.</returns>
    Task<List<StateResponse>> GetAllAsync(StateRequest query, CancellationToken cancellationToken = default);
}