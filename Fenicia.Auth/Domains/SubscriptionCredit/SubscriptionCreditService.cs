using Fenicia.Auth.Domains.Subscription;
using Fenicia.Common;
using Fenicia.Common.Enums;

namespace Fenicia.Auth.Domains.SubscriptionCredit;

public class SubscriptionCreditService(ILogger<SubscriptionCreditService> logger, ISubscriptionCreditRepository subscriptionCreditRepository, ISubscriptionService subscriptionService) : ISubscriptionCreditService
{
    public async Task<ApiResponse<List<ModuleType>>> GetActiveModulesTypesAsync(Guid companyId, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            logger.LogInformation("Getting active modules types for company {CompanyID}", companyId);

            var validSubscriptions = await subscriptionService.GetValidSubscriptionsAsync(companyId, cancellationToken);

            if (validSubscriptions.Data is null)
            {
                logger.LogWarning("No valid subscriptions found for company {CompanyID}", companyId);

                return new ApiResponse<List<ModuleType>>(data: null, validSubscriptions.Status, validSubscriptions.Message?.Message ?? string.Empty);
            }

            logger.LogDebug("Found {Count} valid subscriptions for company {CompanyID}", validSubscriptions.Data.Count, companyId);

            var validModules = await subscriptionCreditRepository.GetValidModulesTypesAsync(validSubscriptions.Data, cancellationToken);

            logger.LogInformation("Retrieved {Count} active module types for company {CompanyID}", validModules.Count, companyId);

            return new ApiResponse<List<ModuleType>>(validModules);
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Operation was cancelled while getting active modules for company {CompanyID}", companyId);

            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting active modules for company {CompanyID}", companyId);

            throw;
        }
    }
}
