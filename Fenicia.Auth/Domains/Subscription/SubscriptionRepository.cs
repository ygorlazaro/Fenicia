namespace Fenicia.Auth.Domains.Subscription;

using Common.Database.Contexts;
using Fenicia.Common.Database.Models.Auth;
using Common.Enums;

using Microsoft.EntityFrameworkCore;

public class SubscriptionRepository : ISubscriptionRepository
{
    private readonly AuthContext _authContext;
    private readonly ILogger<SubscriptionRepository> _logger;

    public SubscriptionRepository(AuthContext authContext, ILogger<SubscriptionRepository> logger)
    {
        _authContext = authContext;
        _logger = logger;
    }

    public async Task SaveSubscriptionAsync(SubscriptionModel subscription, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Saving subscription for company {CompanyId}", subscription.CompanyId);
            _authContext.Subscriptions.Add(subscription);
            await _authContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Successfully saved subscription {SubscriptionId} for company {CompanyId}", subscription.Id, subscription.CompanyId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving subscription for company {CompanyId}", subscription.CompanyId);
            throw;
        }
    }

    public async Task<List<Guid>> GetValidSubscriptionAsync(Guid companyId, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Retrieving valid subscriptions for company {CompanyId}", companyId);
            var now = DateTime.UtcNow;

            var subscriptions = from subscription in _authContext.Subscriptions where subscription.CompanyId == companyId && now >= subscription.StartDate && now <= subscription.EndDate && subscription.Status == SubscriptionStatus.Active select subscription.Id;

            var result = await subscriptions.ToListAsync(cancellationToken);
            _logger.LogInformation("Found {Count} valid subscriptions for company {CompanyId}", result.Count, companyId);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving valid subscriptions for company {CompanyId}", companyId);
            throw;
        }
    }
}
