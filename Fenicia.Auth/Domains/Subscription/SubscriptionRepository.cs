namespace Fenicia.Auth.Domains.Subscription;

using Common.Database.Contexts;
using Fenicia.Common.Database.Models.Auth;
using Common.Enums;

using Microsoft.EntityFrameworkCore;

public class SubscriptionRepository(AuthContext context, ILogger<SubscriptionRepository> logger) : ISubscriptionRepository
{
    public async Task SaveSubscriptionAsync(SubscriptionModel subscription, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Saving subscription for company {CompanyID}", subscription.CompanyId);

            context.Subscriptions.Add(subscription);
            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Successfully saved subscription {SubscriptionID} for company {CompanyID}", subscription.Id, subscription.CompanyId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error saving subscription for company {CompanyID}", subscription.CompanyId);

            throw;
        }
    }

    public async Task<List<Guid>> GetValidSubscriptionAsync(Guid companyId, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Retrieving valid subscriptions for company {CompanyID}", companyId);

            var now = DateTime.UtcNow;
            var subscriptions = context.Subscriptions.Where(subscription => subscription.CompanyId == companyId && now >= subscription.StartDate && now <= subscription.EndDate && subscription.Status == SubscriptionStatus.Active).Select(subscription => subscription.Id);
            var result = await subscriptions.ToListAsync(cancellationToken);

            logger.LogInformation("Found {Count} valid subscriptions for company {CompanyID}", result.Count, companyId);

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving valid subscriptions for company {CompanyID}", companyId);

            throw;
        }
    }
}
