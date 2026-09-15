using Fenicia.Auth.Domains.Subscription.Interfaces;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;
using Fenicia.Common.Enums.Auth;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Subscription;

public class SubscriptionRepository(DbContext context)
    : Repository<SubscriptionModel>(context), ISubscriptionRepository
{
    public Task<List<SubscriptionModel>> GetUserSubscriptionsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        return DbSet
            .Include(s => s.Company)
            .Where(s => s.Company.UsersRoles.Any(ur => ur.UserId == userId))
            .Where(s => s.Status == SubscriptionStatus.Active && now >= s.StartDate && now <= s.EndDate)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    public Task<List<ModuleModel>> GetSubscriptionModulesAsync(
        Guid subscriptionId,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        return DbSet
            .Where(s => s.Id == subscriptionId)
            .SelectMany(s => s.Credits)
            .Where(sc => sc.IsActive && now >= sc.StartDate && now <= sc.EndDate)
            .Select(sc => sc.Module)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    public Task<List<SubscriptionModel>> GetActiveSubscriptionsByCompanyAsync(Guid companyId, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        var query = from s in DbSet
            where s.CompanyId == companyId
                  && s.Status == SubscriptionStatus.Active
                  && now >= s.StartDate
                  && now <= s.EndDate
            select s;

        return query.ToListAsync(cancellationToken);
    }
}