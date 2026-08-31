using Fenicia.Auth.Domains.Module.DTOs;
using Fenicia.Auth.Domains.Subscription;
using Fenicia.Auth.Domains.UserRole;
using Fenicia.Common;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;

namespace Fenicia.Auth.Domains.Module;

public class ModuleService(ModuleRepository repository, UserRoleService userRoleService, SubscriptionService subscriptionService)
{
    public async Task<Pagination<List<GetModuleResponse>>> GetAllModulesAsync(int page, int perPage, CancellationToken ct)
    {
        var modules = await repository.GetAllActiveAsync(page, perPage, ct);
        var total = await repository.CountAllActiveAsync(ct);

        return new Pagination<List<GetModuleResponse>>(modules.Select(m => m.MapToGetModuleResponse()).ToList(), total, page, perPage);
    }

    public async Task<List<GetUserModulesResponse>> GetUserModulesAsync(Guid companyId, Guid userId, CancellationToken ct)
    {
        var userRole = await userRoleService.GetUserRoleAsync(userId, companyId, ct);

        if (userRole is null)
        {
            return [];
        }

        var companySubscriptions = await subscriptionService.GetActiveSubscriptionsByCompanyAsync(companyId, ct);

        var moduleIds = new HashSet<Guid>();

        foreach (var subscription in companySubscriptions)
        {
            var modules = await subscriptionService.GetActiveModulesForSubscriptionAsync(subscription.Id, ct);

            foreach (var module in modules)
            {
                moduleIds.Add(module.Id);
            }
        }

        var modulesResult = await repository.GetByIdsAsync(moduleIds, ct);

        return modulesResult.Select(m => m.MapToGetUserModulesResponse()).ToList();
    }

    public async Task<List<ModuleModel>> GetModulesByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct)
    {
        return await repository.GetByIdsAsync(ids, ct);
    }

    public async Task<ModuleModel?> GetModuleByTypeAsync(ModuleType type, CancellationToken ct)
    {
        return await repository.GetByTypeAsync(type, ct);
    }
}
