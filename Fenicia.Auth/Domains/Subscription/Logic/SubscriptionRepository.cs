using Fenicia.Auth.Contexts;
using Fenicia.Auth.Domains.Subscription.Data;
using Fenicia.Auth.Enums;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Subscription.Logic;

public class SubscriptionRepository(AuthContext authContext) : ISubscriptionRepository
{
    public async Task SaveSubscription(SubscriptionModel subscription,
        CancellationToken cancellationToken)
    {
        authContext.Subscriptions.Add(subscription);

        await authContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<Guid>> GetValidSubscriptionAsync(Guid companyId,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        var subscriptions =
            from subscription in authContext.Subscriptions
            where
                subscription.CompanyId == companyId
                && now >= subscription.StartDate
                && now <= subscription.EndDate
                && subscription.Status == SubscriptionStatus.Active
            select subscription.Id;

        return await subscriptions.ToListAsync(cancellationToken);
    }
}
