using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;
using Fenicia.Common.Enums.Auth;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Subscription;

public class SubscriptionRepository(DefaultContext context) : Repository<SubscriptionModel>(context)
{
    public async Task<List<UserRoleModel>> GetUserRolesAsync(Guid userId, CancellationToken ct = default)
    {
        return await Context.AuthUserRoles
            .Where(ur => ur.UserId == userId)
            .ToListAsync(ct);
    }

    public async Task<List<SubscriptionModel>> GetUserSubscriptionsAsync(Guid userId, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;

        return await DbSet
            .Where(s => s.Company.UsersRoles.Any(ur => ur.UserId == userId))
            .Where(s => s.Status == SubscriptionStatus.Active && now >= s.StartDate && now <= s.EndDate)
            .Distinct()
            .ToListAsync(ct);
    }

    public async Task<List<ModuleModel>> GetSubscriptionModulesAsync(Guid subscriptionId, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;

        return await DbSet
            .Where(s => s.Id == subscriptionId)
            .SelectMany(s => s.Credits)
            .Where(sc => sc.IsActive && now >= sc.StartDate && now <= sc.EndDate)
            .Select(sc => sc.Module)
            .Distinct()
            .ToListAsync(ct);
    }
}
