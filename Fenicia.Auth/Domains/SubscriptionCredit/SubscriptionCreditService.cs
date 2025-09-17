namespace Fenicia.Auth.Domains.SubscriptionCredit;

using Common;
using Common.Enums;

using Fenicia.Auth.Domains.Subscription;

public class SubscriptionCreditService : ISubscriptionCreditService
{
    private readonly ILogger<SubscriptionCreditService> logger;
    private readonly ISubscriptionCreditRepository subscriptionCreditRepository;
    private readonly ISubscriptionService subscriptionService;

    public SubscriptionCreditService(ILogger<SubscriptionCreditService> logger, ISubscriptionCreditRepository subscriptionCreditRepository, ISubscriptionService subscriptionService)
    {
        this.logger = logger;
        this.subscriptionCreditRepository = subscriptionCreditRepository;
        this.subscriptionService = subscriptionService;
    }

    public async Task<ApiResponse<List<ModuleType>>> GetActiveModulesTypesAsync(Guid companyId, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            this.logger.LogInformation("Getting active modules types for company {CompanyID}", companyId);

            var validSubscriptions = await this.subscriptionService.GetValidSubscriptionsAsync(companyId, cancellationToken);

            if (validSubscriptions.Data is null)
            {
                this.logger.LogWarning("No valid subscriptions found for company {CompanyID}", companyId);
                return new ApiResponse<List<ModuleType>>(data: null, validSubscriptions.Status, validSubscriptions.Message.Message ?? string.Empty);
            }

            this.logger.LogDebug("Found {Count} valid subscriptions for company {CompanyID}", validSubscriptions.Data.Count, companyId);

            var validModules = await this.subscriptionCreditRepository.GetValidModulesTypesAsync(validSubscriptions.Data, cancellationToken);

            this.logger.LogInformation("Retrieved {Count} active module types for company {CompanyID}", validModules.Count, companyId);
            return new ApiResponse<List<ModuleType>>(validModules);
        }
        catch (OperationCanceledException)
        {
            this.logger.LogWarning("Operation was cancelled while getting active modules for company {CompanyID}", companyId);
            throw;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error getting active modules for company {CompanyID}", companyId);
            throw;
        }
    }
}
