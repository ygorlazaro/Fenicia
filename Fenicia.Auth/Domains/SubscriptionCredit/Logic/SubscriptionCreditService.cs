namespace Fenicia.Auth.Domains.SubscriptionCredit.Logic;

using Common;
using Common.Enums;

using Subscription.Logic;

/// <summary>
///     Service responsible for managing subscription credits and module access
/// </summary>
public class SubscriptionCreditService(ILogger<SubscriptionCreditService> logger, ISubscriptionCreditRepository subscriptionCreditRepository, ISubscriptionService subscriptionService) : ISubscriptionCreditService
{
    /// <summary>
    ///     Retrieves active module types for a specific company
    /// </summary>
    /// <param name="companyId">The unique identifier of the company</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation</param>
    /// <returns>A list of active module types wrapped in an ApiResponse</returns>
    public async Task<ApiResponse<List<ModuleType>>> GetActiveModulesTypesAsync(Guid companyId, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            logger.LogInformation(message: "Getting active modules types for company {CompanyId}", companyId);

            var validSubscriptions = await subscriptionService.GetValidSubscriptionsAsync(companyId, cancellationToken);

            if (validSubscriptions.Data is null)
            {
                logger.LogWarning(message: "No valid subscriptions found for company {CompanyId}", companyId);
                return new ApiResponse<List<ModuleType>>(data: null, validSubscriptions.Status, validSubscriptions.Message);
            }

            logger.LogDebug(message: "Found {Count} valid subscriptions for company {CompanyId}", validSubscriptions.Data.Count, companyId);

            var validModules = await subscriptionCreditRepository.GetValidModulesTypesAsync(validSubscriptions.Data, cancellationToken);

            logger.LogInformation(message: "Retrieved {Count} active module types for company {CompanyId}", validModules.Count, companyId);
            return new ApiResponse<List<ModuleType>>(validModules);
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning(message: "Operation was cancelled while getting active modules for company {CompanyId}", companyId);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Error getting active modules for company {CompanyId}", companyId);
            throw;
        }
    }
}
