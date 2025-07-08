namespace Fenicia.Auth.Domains.Subscription.Logic;

using Data;

public interface ISubscriptionRepository
{
    Task SaveSubscriptionAsync(SubscriptionModel subscription, CancellationToken cancellationToken);

    Task<List<Guid>> GetValidSubscriptionAsync(Guid companyId, CancellationToken cancellationToken);
}
