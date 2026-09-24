using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;
using Fenicia.Common.Enums.Auth;

namespace Fenicia.Auth.Domains.Module.Interfaces;

/// <summary>
/// Represents a repository for managing modules in the authentication domain.
/// </summary>
public interface IModuleRepository : IRepository<ModuleModel>
{
    /// <summary>
    /// Gets a list of modules by their unique identifiers (IDs).
    /// </summary>
    /// <param name="ids">The unique identifiers (IDs) of the modules to retrieve.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<List<ModuleModel>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a module by its type.
    /// </summary>
    /// <param name="type">The type of the module to retrieve.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<ModuleModel?> GetByTypeAsync(EnumModuleType type, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a list of active modules.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<List<ModuleModel>> GetActiveModulesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total number of active modules.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<int> GetTotalActiveModulesAsync(CancellationToken cancellationToken = default);
}
