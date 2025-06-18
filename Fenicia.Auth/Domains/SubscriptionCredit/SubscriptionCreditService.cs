using Fenicia.Auth.Domains.Subscription;
using Fenicia.Common;
using Fenicia.Common.Enums;

namespace Fenicia.Auth.Domains.SubscriptionCredit;

public class SubscriptionCreditService(
    ILogger<SubscriptionCreditService> logger,
    ISubscriptionCreditRepository subscriptionCreditRepository,
    ISubscriptionService subscriptionService
) : ISubscriptionCreditService
{
    public async Task<ApiResponse<List<ModuleType>>> GetActiveModulesTypesAsync(Guid companyId)
    {
        logger.LogInformation("Getting active modules types");

        var validSubscriptions = await subscriptionService.GetValidSubscriptionsAsync(companyId);

        if (validSubscriptions.Data is null)
        {
            return new ApiResponse<List<ModuleType>>(
                null,
                validSubscriptions.StatusCode,
                validSubscriptions.Message
            );
        }

        var validModules = await subscriptionCreditRepository.GetValidModulesTypesAsync(
            validSubscriptions.Data
        );

        return new ApiResponse<List<ModuleType>>(validModules);
    }
}
