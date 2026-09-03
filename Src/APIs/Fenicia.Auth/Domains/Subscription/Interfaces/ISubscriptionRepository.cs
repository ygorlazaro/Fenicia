using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;

namespace Fenicia.Auth.Domains.Subscription.Interfaces;

public interface ISubscriptionRepository : IRepository<SubscriptionModel>
{
    Task<List<SubscriptionModel>> GetUserSubscriptionsAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<List<ModuleModel>> GetSubscriptionModulesAsync(
        Guid subscriptionId,
        CancellationToken cancellationToken = default);
}