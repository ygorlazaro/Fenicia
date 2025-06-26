using Fenicia.Auth.Contexts;
using Fenicia.Auth.Domains.Subscription.Data;
using Fenicia.Auth.Enums;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Subscription.Logic;

/// <summary>
/// Repository for managing subscription data operations
/// </summary>
public class SubscriptionRepository(AuthContext authContext, ILogger<SubscriptionRepository> logger) : ISubscriptionRepository
{
    /// <summary>
    /// Saves a new subscription to the database
    /// </summary>
    /// <param name="subscription">The subscription model to save</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public async Task SaveSubscriptionAsync(SubscriptionModel subscription,
        CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Saving subscription for company {CompanyId}", subscription.CompanyId);
            authContext.Subscriptions.Add(subscription);
            await authContext.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Successfully saved subscription {SubscriptionId} for company {CompanyId}",
                subscription.Id, subscription.CompanyId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error saving subscription for company {CompanyId}", subscription.CompanyId);
            throw;
        }
    }

    /// <summary>
    /// Retrieves valid subscriptions for a company
    /// </summary>
    /// <param name="companyId">The ID of the company</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of valid subscription IDs</returns>
    public async Task<List<Guid>> GetValidSubscriptionAsync(Guid companyId,
        CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Retrieving valid subscriptions for company {CompanyId}", companyId);
            var now = DateTime.UtcNow;

            var subscriptions =
                from subscription in authContext.Subscriptions
                where
                    subscription.CompanyId == companyId
                    && now >= subscription.StartDate
                    && now <= subscription.EndDate
                    && subscription.Status == SubscriptionStatus.Active
                select subscription.Id;

            var result = await subscriptions.ToListAsync(cancellationToken);
            logger.LogInformation("Found {Count} valid subscriptions for company {CompanyId}",
                result.Count, companyId);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving valid subscriptions for company {CompanyId}", companyId);
            throw;
        }
    }
}
