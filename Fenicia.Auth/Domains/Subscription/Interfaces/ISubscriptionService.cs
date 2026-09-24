using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.Subscription;

namespace Fenicia.Auth.Domains.Subscription.Interfaces;

/// <summary>
/// Service interface for managing subscriptions in the authentication domain.
/// </summary>
public interface ISubscriptionService
{
    /// <summary>
    /// Gets the user profile with companies and subscriptions.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with the user profile.</returns>
    Task<SubscriptionResponse?> GetUserProfileAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new subscription.
    /// </summary>
    /// <param name="subscription">The subscription model to create.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task CreateSubscriptionAsync(SubscriptionModel subscription, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active subscriptions for a specific company.
    /// </summary>
    /// <param name="companyId">The unique identifier of the company.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with a list of subscriptions.</returns>
    Task<List<SubscriptionModel>> GetActiveSubscriptionsByCompanyAsync(
        Guid companyId,
        CancellationToken cancellationToken = default);
}