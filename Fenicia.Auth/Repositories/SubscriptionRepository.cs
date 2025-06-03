using Fenicia.Auth.Contexts;
using Fenicia.Auth.Contexts.Models;
using Fenicia.Auth.Enums;
using Fenicia.Auth.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Repositories;

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

        var subscriptions = from subscription in authContext.Subscriptions
                            where subscription.CompanyId == companyId
                                  && now >= subscription.StartDate
                                  && now <= subscription.EndDate
                                  && subscription.Status == SubscriptionStatus.Active
                            select subscription.Id;

        return await subscriptions.ToListAsync();
    }
}