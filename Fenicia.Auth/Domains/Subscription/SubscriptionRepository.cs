namespace Fenicia.Auth.Domains.Subscription;

using Common.Database.Contexts;
using Fenicia.Common.Database.Models.Auth;
using Common.Enums;

using Microsoft.EntityFrameworkCore;

public class SubscriptionRepository : ISubscriptionRepository
{
    private readonly AuthContext authContext;
    private readonly ILogger<SubscriptionRepository> logger;

    public SubscriptionRepository(AuthContext authContext, ILogger<SubscriptionRepository> logger)
    {
        this.authContext = authContext;
        this.logger = logger;
    }

    public async Task SaveSubscriptionAsync(SubscriptionModel subscription, CancellationToken cancellationToken)
    {
        try
        {
            this.logger.LogInformation("Saving subscription for company {CompanyID}", subscription.CompanyId);
            this.authContext.Subscriptions.Add(subscription);
            await this.authContext.SaveChangesAsync(cancellationToken);
            this.logger.LogInformation("Successfully saved subscription {SubscriptionID} for company {CompanyID}", subscription.Id, subscription.CompanyId);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error saving subscription for company {CompanyID}", subscription.CompanyId);
            throw;
        }
    }

    public async Task<List<Guid>> GetValidSubscriptionAsync(Guid companyId, CancellationToken cancellationToken)
    {
        try
        {
            this.logger.LogInformation("Retrieving valid subscriptions for company {CompanyID}", companyId);
            var now = DateTime.UtcNow;

            var subscriptions = from subscription in this.authContext.Subscriptions where subscription.CompanyId == companyId && now >= subscription.StartDate && now <= subscription.EndDate && subscription.Status == SubscriptionStatus.Active select subscription.Id;

            var result = await subscriptions.ToListAsync(cancellationToken);
            this.logger.LogInformation("Found {Count} valid subscriptions for company {CompanyID}", result.Count, companyId);
            return result;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error retrieving valid subscriptions for company {CompanyID}", companyId);
            throw;
        }
    }
}
