using Fenicia.Common.Database.Models.Auth;

namespace Fenicia.Auth.Domains.Subscription;

public interface ISubscriptionRepository
{
    Task SaveSubscriptionAsync(SubscriptionModel subscription, CancellationToken cancellationToken);

    Task<List<Guid>> GetValidSubscriptionAsync(Guid companyId, CancellationToken cancellationToken);
}
