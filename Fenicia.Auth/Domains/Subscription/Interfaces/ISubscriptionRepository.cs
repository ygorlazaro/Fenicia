using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;

namespace Fenicia.Auth.Domains.Subscription.Interfaces;

/// <summary>
/// Repository interface for managing subscriptions in the authentication domain.
/// </summary>
public interface ISubscriptionRepository : IRepository<SubscriptionModel>
{
    /// <summary>
    /// Gets all subscriptions for a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with a list of subscriptions.</returns>
    Task<List<SubscriptionModel>> GetUserSubscriptionsAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active subscriptions for a specific company.
    /// </summary>
    /// <param name="companyId">The unique identifier of the company.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with a list of subscriptions.</returns>
    Task<List<SubscriptionModel>> GetActiveSubscriptionsByCompanyAsync(Guid companyId, CancellationToken cancellationToken);
}