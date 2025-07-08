namespace Fenicia.Auth.Domains.Subscription.Logic;

using Contexts;

using Data;

using Enums;

using Microsoft.EntityFrameworkCore;

public class SubscriptionRepository(AuthContext authContext, ILogger<SubscriptionRepository> logger) : ISubscriptionRepository
{
    public async Task SaveSubscriptionAsync(SubscriptionModel subscription, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation(message: "Saving subscription for company {CompanyId}", subscription.CompanyId);
            authContext.Subscriptions.Add(subscription);
            await authContext.SaveChangesAsync(cancellationToken);
            logger.LogInformation(message: "Successfully saved subscription {SubscriptionId} for company {CompanyId}", subscription.Id, subscription.CompanyId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Error saving subscription for company {CompanyId}", subscription.CompanyId);
            throw;
        }
    }

    public async Task<List<Guid>> GetValidSubscriptionAsync(Guid companyId, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation(message: "Retrieving valid subscriptions for company {CompanyId}", companyId);
            var now = DateTime.UtcNow;

            var subscriptions = from subscription in authContext.Subscriptions where subscription.CompanyId == companyId && now >= subscription.StartDate && now <= subscription.EndDate && subscription.Status == SubscriptionStatus.Active select subscription.Id;

            var result = await subscriptions.ToListAsync(cancellationToken);
            logger.LogInformation(message: "Found {Count} valid subscriptions for company {CompanyId}", result.Count, companyId);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Error retrieving valid subscriptions for company {CompanyId}", companyId);
            throw;
        }
    }
}
