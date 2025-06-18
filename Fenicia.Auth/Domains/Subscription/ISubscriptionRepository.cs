namespace Fenicia.Auth.Domains.Subscription;

public interface ISubscriptionRepository
{
    Task SaveSubscription(SubscriptionModel subscription);
    Task<List<Guid>> GetValidSubscriptionAsync(Guid companyId);
}
