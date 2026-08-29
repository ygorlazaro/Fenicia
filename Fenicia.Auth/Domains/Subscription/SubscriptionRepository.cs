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

        var query = from ur in Context.AuthUserRoles
                    join c in Context.AuthCompanies on ur.CompanyId equals c.Id
                    join s in Context.AuthSubscriptions on c.Id equals s.CompanyId
                    where ur.UserId == userId &&
                          s.Status == SubscriptionStatus.Active &&
                          now >= s.StartDate &&
                          now <= s.EndDate
                    select s;

        return await query.Distinct().ToListAsync(ct);
    }

    public async Task<List<ModuleModel>> GetSubscriptionModulesAsync(Guid subscriptionId, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;

        var query = from sc in Context.AuthSubscriptionCredits
                    join m in Context.AuthModules on sc.ModuleId equals m.Id
                    where sc.SubscriptionId == subscriptionId &&
                          sc.IsActive &&
                          now >= sc.StartDate &&
                          now <= sc.EndDate
                    select m;

        return await query.Distinct().ToListAsync(ct);
    }
}
