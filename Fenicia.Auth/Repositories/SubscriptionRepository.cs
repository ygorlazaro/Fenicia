using Fenicia.Auth.Contexts;
using Fenicia.Auth.Contexts.Models;
using Fenicia.Auth.Repositories.Interfaces;

namespace Fenicia.Auth.Repositories;

public class SubscriptionRepository(AuthContext authContext) : ISubscriptionRepository
{
    public async Task SaveSubscription(SubscriptionModel subscription)
    {
        authContext.Subscriptions.Add(subscription);

        await authContext.SaveChangesAsync();
    }
}