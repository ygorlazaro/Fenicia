namespace Fenicia.Auth.Domains.Subscription.Logic;

using Data;

/// <summary>
///     Represents a repository for managing subscription-related operations.
/// </summary>
public interface ISubscriptionRepository
{
    /// <summary>
    ///     Asynchronously saves a subscription to the repository.
    /// </summary>
    /// <param name="subscription">The subscription model to save.</param>
    /// <param name="cancellationToken">A token to cancel the operation if needed.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when subscription is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the save operation fails.</exception>
    Task SaveSubscriptionAsync(SubscriptionModel subscription, CancellationToken cancellationToken);

    /// <summary>
    ///     Asynchronously retrieves a list of valid subscription IDs for a specific company.
    /// </summary>
    /// <param name="companyId">The ID of the company to get subscriptions for.</param>
    /// <param name="cancellationToken">A token to cancel the operation if needed.</param>
    /// <returns>A task representing the asynchronous operation, containing a list of subscription IDs.</returns>
    /// <exception cref="ArgumentException">Thrown when companyId is empty.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the retrieval operation fails.</exception>
    Task<List<Guid>> GetValidSubscriptionAsync(Guid companyId, CancellationToken cancellationToken);
}
