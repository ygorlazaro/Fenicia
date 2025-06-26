using Fenicia.Auth.Domains.Module.Data;
using Fenicia.Common;
using Fenicia.Common.Enums;

namespace Fenicia.Auth.Domains.Module.Logic;

/// <summary>
/// Provides services for managing modules in the system
/// </summary>
public interface IModuleService
{
    /// <summary>
    /// Retrieves a paginated list of all modules ordered by default criteria
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to cancel the operation if needed</param>
    /// <param name="page">The page number to retrieve (starting from 1)</param>
    /// <param name="perPage">The number of items per page</param>
    /// <returns>ApiResponse containing a list of ModuleResponse objects</returns>
    Task<ApiResponse<List<ModuleResponse>>> GetAllOrderedAsync(CancellationToken cancellationToken,
        int page = 1, int perPage = 10);

    /// <summary>
    /// Retrieves modules based on the provided module IDs
    /// </summary>
    /// <param name="request">Collection of module IDs to retrieve</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation if needed</param>
    /// <returns>ApiResponse containing a list of ModuleResponse objects</returns>
    Task<ApiResponse<List<ModuleResponse>>> GetModulesToOrderAsync(IEnumerable<Guid> request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a module by its type
    /// </summary>
    /// <param name="moduleType">The type of module to retrieve</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation if needed</param>
    /// <returns>ApiResponse containing a ModuleResponse object</returns>
    Task<ApiResponse<ModuleResponse>> GetModuleByTypeAsync(ModuleType moduleType,
        CancellationToken cancellationToken);

    /// <summary>
    /// Counts the total number of modules in the system
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to cancel the operation if needed</param>
    /// <returns>ApiResponse containing the total count of modules</returns>
    Task<ApiResponse<int>> CountAsync(CancellationToken cancellationToken);
}
