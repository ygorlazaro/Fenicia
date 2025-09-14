namespace Fenicia.Auth.Domains.Subscription;

using Fenicia.Common.Database.Models.Auth;

public interface ISubscriptionRepository
{
    Task SaveSubscriptionAsync(SubscriptionModel subscription, CancellationToken cancellationToken);

    Task<List<Guid>> GetValidSubscriptionAsync(Guid companyId, CancellationToken cancellationToken);
}
