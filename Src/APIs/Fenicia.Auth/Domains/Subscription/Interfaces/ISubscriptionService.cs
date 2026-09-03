using Fenicia.Auth.Domains.Subscription.DTOs;
using Fenicia.Common.Data.Models.Auth;

namespace Fenicia.Auth.Domains.Subscription.Interfaces;

public interface ISubscriptionService
{
    Task<GetUserProfileResponse?> GetUserProfileAsync(Guid userId, CancellationToken cancellationToken = default);

    Task CreateSubscriptionAsync(SubscriptionModel subscription, CancellationToken cancellationToken = default);

    Task<List<ModuleModel>> GetActiveModulesForSubscriptionAsync(
        Guid subscriptionId,
        CancellationToken cancellationToken = default);

    Task<List<SubscriptionModel>> GetActiveSubscriptionsByCompanyAsync(
        Guid companyId,
        CancellationToken cancellationToken = default);
}