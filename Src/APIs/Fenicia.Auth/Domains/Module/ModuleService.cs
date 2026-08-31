using Fenicia.Auth.Domains.Module.DTOs;
using Fenicia.Auth.Domains.Subscription;
using Fenicia.Auth.Domains.UserRole;
using Fenicia.Common;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Module;

public class ModuleService(ModuleRepository repository, UserRoleService userRoleService, SubscriptionService subscriptionService)
{
    public async Task<Pagination<List<GetModuleResponse>>> GetAllModulesAsync(PaginationQuery query, CancellationToken cancellationToken = default)
    {
        var baseQuery = repository.Query().Where(m => m.Type != ModuleType.Auth && m.IsActive);
        var filters = AdvancedQueryParser.Parse(query.Query);
        var filteredQuery = baseQuery.ApplyAdvancedQuery(filters, query.Sort);
        var orderedQuery = filteredQuery.OrderBy(m => m.SortOrder);

        var totalTask = orderedQuery.CountAsync(cancellationToken);
        var modulesTask = orderedQuery.Skip((query.Page - 1) * query.PerPage).Take(query.PerPage).ToListAsync(cancellationToken);

        await Task.WhenAll(totalTask, modulesTask);

        return new Pagination<List<GetModuleResponse>>([.. modulesTask.Result.Select(m => m.MapToGetModuleResponse())], totalTask.Result, query.Page, query.PerPage);
    }

    public async Task<List<GetUserModulesResponse>> GetUserModulesAsync(Guid companyId, Guid userId, CancellationToken cancellationToken = default)
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

    public async Task<List<ModuleModel>> GetModulesByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        return await repository.GetByIdsAsync(ids, cancellationToken);
    }

    public async Task<ModuleModel?> GetModuleByTypeAsync(ModuleType type, CancellationToken cancellationToken = default)
    {
        return await repository.GetByTypeAsync(type, cancellationToken);
    }
}
