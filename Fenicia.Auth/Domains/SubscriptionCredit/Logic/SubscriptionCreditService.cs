using Fenicia.Auth.Domains.Subscription.Logic;
using Fenicia.Common;
using Fenicia.Common.Enums;

namespace Fenicia.Auth.Domains.SubscriptionCredit.Logic;

public class SubscriptionCreditService(
    ILogger<SubscriptionCreditService> logger,
    ISubscriptionCreditRepository subscriptionCreditRepository,
    ISubscriptionService subscriptionService
) : ISubscriptionCreditService
{
    public async Task<ApiResponse<List<ModuleType>>> GetActiveModulesTypesAsync(Guid companyId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting active modules types");

        var validSubscriptions = await subscriptionService.GetValidSubscriptionsAsync(companyId, cancellationToken);

        if (validSubscriptions.Data is null)
        {
            return new ApiResponse<List<ModuleType>>(
                null,
                validSubscriptions.Status,
                validSubscriptions.Message
            );
        }

        var validModules = await subscriptionCreditRepository.GetValidModulesTypesAsync(
            validSubscriptions.Data,
            cancellationToken
        );

        return new ApiResponse<List<ModuleType>>(validModules);
    }
}
