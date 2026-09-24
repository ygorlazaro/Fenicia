using Fenicia.Common;
using Fenicia.Common.DTOs.Auth.Module;
using Fenicia.Common.Enums.Auth;

namespace Fenicia.Auth.Domains.Module.Interfaces;

/// <summary>
/// Represents a service for managing modules in the authentication domain.
/// </summary>
public interface IModuleService
{
    /// <summary>
    /// Gets a paginated list of all modules based on the provided query parameters.
    /// </summary>
    /// <param name="query">The query parameters.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<Pagination<IEnumerable<ModuleResponse>>> GetAllModulesAsync(
        PaginationQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a list of modules associated with a specific user and company.
    /// </summary>
    /// <param name="companyId">The unique identifier (ID) of the company.</param>
    /// <param name="userId">The unique identifier (ID) of the user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<IEnumerable<ModuleByUserResponse>> GetUserModulesAsync(
        Guid companyId,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a list of modules by their unique identifiers (IDs).
    /// </summary>
    /// <param name="ids">The unique identifiers (IDs) of the modules to retrieve.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<IEnumerable<ModuleResponse>> GetModulesByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a module by its type.
    /// </summary>
    /// <param name="type">The type of the module to retrieve.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<ModuleResponse?> GetModuleByTypeAsync(
        EnumModuleType type,
        CancellationToken cancellationToken = default);
}
