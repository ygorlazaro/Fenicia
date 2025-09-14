namespace Fenicia.Auth.Domains.SubscriptionCredit.Logic;

using Common;
using Common.Enums;

using Subscription.Logic;

public class SubscriptionCreditService : ISubscriptionCreditService
{
    private readonly ILogger<SubscriptionCreditService> _logger;
    private readonly ISubscriptionCreditRepository _subscriptionCreditRepository;
    private readonly ISubscriptionService _subscriptionService;

    public SubscriptionCreditService(ILogger<SubscriptionCreditService> logger, ISubscriptionCreditRepository subscriptionCreditRepository, ISubscriptionService subscriptionService)
    {
        _logger = logger;
        _subscriptionCreditRepository = subscriptionCreditRepository;
        _subscriptionService = subscriptionService;
    }

    public async Task<ApiResponse<List<ModuleType>>> GetActiveModulesTypesAsync(Guid companyId, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            _logger.LogInformation("Getting active modules types for company {CompanyId}", companyId);

            var validSubscriptions = await _subscriptionService.GetValidSubscriptionsAsync(companyId, cancellationToken);

            if (validSubscriptions.Data is null)
            {
                _logger.LogWarning("No valid subscriptions found for company {CompanyId}", companyId);
                return new ApiResponse<List<ModuleType>>(data: null, validSubscriptions.Status, validSubscriptions.Message.Message ?? string.Empty);
            }

            _logger.LogDebug("Found {Count} valid subscriptions for company {CompanyId}", validSubscriptions.Data.Count, companyId);

            var validModules = await _subscriptionCreditRepository.GetValidModulesTypesAsync(validSubscriptions.Data, cancellationToken);

            _logger.LogInformation("Retrieved {Count} active module types for company {CompanyId}", validModules.Count, companyId);
            return new ApiResponse<List<ModuleType>>(validModules);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Operation was cancelled while getting active modules for company {CompanyId}", companyId);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active modules for company {CompanyId}", companyId);
            throw;
        }
    }
}
