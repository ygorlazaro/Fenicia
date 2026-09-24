using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;

namespace Fenicia.Auth.Domains.State.Interfaces;

/// <summary>
/// Defines the contract for a repository that manages state-related data operations.
/// </summary>
public interface IStateRepository : IRepository<StateModel>
{
    /// <summary>
    /// Retrieves all states ordered by UF.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of all states ordered by UF.</returns>
    Task<List<StateModel>> GetAllOrderedAsync(CancellationToken cancellationToken = default);
}