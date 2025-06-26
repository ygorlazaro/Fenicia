using Fenicia.Auth.Domains.Subscription.Data;

namespace Fenicia.Auth.Domains.Subscription.Logic;

public interface ISubscriptionRepository
{
    Task SaveSubscription(SubscriptionModel subscription, CancellationToken cancellationToken);
    Task<List<Guid>> GetValidSubscriptionAsync(Guid companyId, CancellationToken cancellationToken);
}
