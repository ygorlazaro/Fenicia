using Fenicia.Auth.Domains.Module.Interfaces;
using Fenicia.Auth.Domains.Subscription.Interfaces;
using Fenicia.Auth.Domains.UserRole.Interfaces;
using Fenicia.Common;
using Fenicia.Common.DTOs.Auth.Module;
using Fenicia.Common.Enums.Auth;

namespace Fenicia.Auth.Domains.Module;

/// <summary>
/// Service class for managing modules, including retrieval of all modules, user-specific modules, and module details by ID or type.
/// </summary>
/// <param name="repository">The module repository.</param>
/// <param name="userRoleService">The user role service.</param>
/// <param name="subscriptionService">The subscription service.</param>
public class ModuleService(
    IModuleRepository repository,
    IUserRoleService userRoleService,
    ISubscriptionService subscriptionService) : IModuleService
{
    /// <summary>
    /// Retrieves a paginated list of all active modules, mapping them to ModuleResponse DTOs.
    /// </summary>
    /// <param name="query">The pagination query.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The paginated list of modules.</returns>
    public async Task<Pagination<IEnumerable<ModuleResponse>>> GetAllModulesAsync(
        PaginationQuery query,
        CancellationToken cancellationToken = default)
    {
        var modules = await repository.GetActiveModulesAsync(cancellationToken);
        var total = await repository.GetTotalActiveModulesAsync(cancellationToken);

        return new Pagination<IEnumerable<ModuleResponse>>(
            modules.Select(ModuleMapper.MapToModuleResponse),
            total,
            query.Page,
            query.PerPage);
    }

    /// <summary>
    /// Retrieves a list of modules available to a specific user based on their role and subscription.
    /// </summary>
    /// <param name="companyId">The company ID.</param>
    /// <param name="userId">The user ID.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The list of modules available to the user.</returns>
    public async Task<IEnumerable<ModuleByUserResponse>> GetUserModulesAsync(
        Guid companyId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var userRole = await userRoleService.GetUserRoleAsync(userId, companyId, cancellationToken);

        if (userRole is null)
        {
            return [];
        }

        var companySubscriptions =
            await subscriptionService.GetActiveSubscriptionsByCompanyAsync(companyId, cancellationToken);

        var moduleIds = new HashSet<Guid>();

        foreach (var subscription in companySubscriptions)
        {
            var modules =
                await GetActiveModulesForSubscriptionAsync(subscription.Id, cancellationToken);

            foreach (var module in modules)
            {
                moduleIds.Add(module.Id);
            }
        }

        var modulesResult = await repository.GetByIdsAsync(moduleIds, cancellationToken);

        return [.. modulesResult.Select(ModuleMapper.MapToModuleByUserResponse)];
    }

    /// <summary>
    /// Retrieves a module by its ID, returning null if not found.
    /// </summary>
    /// <param name="ids">The IDs of the modules to retrieve.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The list of modules.</returns>
    public async Task<IEnumerable<ModuleResponse>> GetModulesByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken = default)
    {
        var modules = await repository.GetByIdsAsync(ids, cancellationToken);

        return modules.Select(ModuleMapper.MapToModuleResponse);
    }

    /// <summary>
    /// Retrieves a module by its type, returning null if not found.
    /// </summary>
    /// <param name="type">The type of the module to retrieve.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The module if found, otherwise null.</returns>
    public async Task<ModuleResponse?> GetModuleByTypeAsync(EnumModuleType type, CancellationToken cancellationToken = default)
    {
        var module = await repository.GetByTypeAsync(type, cancellationToken);
        return module != null ? ModuleMapper.MapToModuleResponse(module) : null;
    }

    /// <summary>
    /// Gets all active modules for a specific subscription.
    /// </summary>
    /// <param name="subscriptionId">The unique identifier of the subscription.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with a list of module responses.</returns>
    public async Task<IEnumerable<ModuleResponse>> GetActiveModulesForSubscriptionAsync(
        Guid subscriptionId,
        CancellationToken cancellationToken = default)
    {
        var modules = await repository.GetSubscriptionModulesAsync(subscriptionId, cancellationToken);
        return modules.Select(ModuleMapper.MapToModuleResponse);
    }
}
