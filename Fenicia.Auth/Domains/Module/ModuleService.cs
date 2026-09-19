using Fenicia.Auth.Domains.Module.Interfaces;
using Fenicia.Auth.Domains.Subscription.Interfaces;
using Fenicia.Auth.Domains.UserRole.Interfaces;
using Fenicia.Common;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.Module;
using Fenicia.Common.Enums.Auth;

namespace Fenicia.Auth.Domains.Module;

public class ModuleService(
    ModuleMapper mapper,
    IModuleRepository repository,
    IUserRoleService userRoleService,
    ISubscriptionService subscriptionService) : IModuleService
{
    public async Task<Pagination<List<ModuleResponse>>> GetAllModulesAsync(
        PaginationQuery query,
        CancellationToken cancellationToken = default)
    {
        var modules = await repository.GetActiveModulesAsync(cancellationToken);
        var total = await repository.GetTotalActiveModulesAsync(cancellationToken);

        return new Pagination<List<ModuleResponse>>(
            [.. modules.Select(mapper.MapToModuleResponse)],
            total,
            query.Page,
            query.PerPage);
    }

    public async Task<List<ModuleByUserResponse>> GetUserModulesAsync(
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
                await subscriptionService.GetActiveModulesForSubscriptionAsync(subscription.Id, cancellationToken);

            foreach (var module in modules)
            {
                moduleIds.Add(module.Id);
            }
        }

        var modulesResult = await repository.GetByIdsAsync(moduleIds, cancellationToken);

        return [.. modulesResult.Select(mapper.MapToModuleByUserResponse)];
    }

    public Task<List<ModuleModel>> GetModulesByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken = default)
    {
        return repository.GetByIdsAsync(ids, cancellationToken);
    }

    public Task<ModuleModel?> GetModuleByTypeAsync(ModuleType type, CancellationToken cancellationToken = default)
    {
        return repository.GetByTypeAsync(type, cancellationToken);
    }
}