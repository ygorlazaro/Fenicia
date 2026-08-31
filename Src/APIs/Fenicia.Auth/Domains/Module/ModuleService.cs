using Fenicia.Auth.Domains.Module.DTOs;
using Fenicia.Auth.Domains.Subscription;
using Fenicia.Auth.Domains.UserRole;
using Fenicia.Common;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;

namespace Fenicia.Auth.Domains.Module;

public class ModuleService(ModuleRepository repository, UserRoleService userRoleService, SubscriptionService subscriptionService)
{
    public async Task<Pagination<List<GetModuleResponse>>> GetAllModulesAsync(int page, int perPage, CancellationToken cancellationToken)
    {
        var modules = await repository.GetAllActiveAsync(page, perPage, cancellationToken);
        var total = await repository.CountAllActiveAsync(cancellationToken);

        return new Pagination<List<GetModuleResponse>>([.. modules.Select(m => m.MapToGetModuleResponse())], total, page, perPage);
    }

    public async Task<List<GetUserModulesResponse>> GetUserModulesAsync(Guid companyId, Guid userId, CancellationToken cancellationToken)
    {
        var userRole = await userRoleService.GetUserRoleAsync(userId, companyId, cancellationToken);

        if (userRole is null)
        {
            return [];
        }

        var companySubscriptions = await subscriptionService.GetActiveSubscriptionsByCompanyAsync(companyId, cancellationToken);

        var moduleIds = new HashSet<Guid>();

        foreach (var subscription in companySubscriptions)
        {
            var modules = await subscriptionService.GetActiveModulesForSubscriptionAsync(subscription.Id, cancellationToken);

            foreach (var module in modules)
            {
                moduleIds.Add(module.Id);
            }
        }

        var modulesResult = await repository.GetByIdsAsync(moduleIds, cancellationToken);

        return [.. modulesResult.Select(m => m.MapToGetUserModulesResponse())];
    }

    public async Task<List<ModuleModel>> GetModulesByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken)
    {
        return await repository.GetByIdsAsync(ids, cancellationToken);
    }

    public async Task<ModuleModel?> GetModuleByTypeAsync(ModuleType type, CancellationToken cancellationToken)
    {
        return await repository.GetByTypeAsync(type, cancellationToken);
    }
}
