using Fenicia.Auth.Contexts;
using Fenicia.Auth.Enums;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Subscription;

public class SubscriptionRepository(AuthContext authContext) : ISubscriptionRepository
{
    public async Task SaveSubscription(SubscriptionModel subscription)
    {
        authContext.Subscriptions.Add(subscription);

        await authContext.SaveChangesAsync();
    }

    public async Task<List<Guid>> GetValidSubscriptionAsync(Guid companyId)
    {
        var now = DateTime.Now;

        var subscriptions =
            from subscription in authContext.Subscriptions
            where
                subscription.CompanyId == companyId
                && now >= subscription.StartDate
                && now <= subscription.EndDate
                && subscription.Status == SubscriptionStatus.Active
            select subscription.Id;

        return await subscriptions.ToListAsync();
    }
}
