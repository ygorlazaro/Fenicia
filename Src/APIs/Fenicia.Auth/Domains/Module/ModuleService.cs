using Fenicia.Auth.Domains.Module.DTOs;
using Fenicia.Auth.Domains.Module.Interfaces;
using Fenicia.Auth.Domains.Subscription.Interfaces;
using Fenicia.Auth.Domains.UserRole.Interfaces;
using Fenicia.Common;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Module;

public class ModuleService(
    IModuleRepository repository,
    IUserRoleService userRoleService,
    ISubscriptionService subscriptionService) : IModuleService
{
    public async Task<Pagination<List<GetModuleResponse>>> GetAllModulesAsync(
        PaginationQuery query,
        CancellationToken cancellationToken = default)
    {
        var baseQuery = repository.Query().Where(m => m.Type != ModuleType.Auth && m.IsActive);
        var filteredQuery = baseQuery;
        var orderedQuery = filteredQuery.OrderBy(m => m.SortOrder);

        var total = await orderedQuery.CountAsync(cancellationToken);
        var modules = await orderedQuery.Skip((query.Page - 1) * query.PerPage).Take(query.PerPage)
            .ToListAsync(cancellationToken);

        return new Pagination<List<GetModuleResponse>>(
            [.. modules.Select(m => m.MapToGetModuleResponse())],
            total,
            query.Page,
            query.PerPage);
    }

    public async Task<List<GetUserModulesResponse>> GetUserModulesAsync(
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

        return [.. modulesResult.Select(m => m.MapToGetUserModulesResponse())];
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