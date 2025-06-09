using Fenicia.Auth.Repositories.Interfaces;
using Fenicia.Auth.Services.Interfaces;
using Fenicia.Common;
using Fenicia.Common.Enums;

namespace Fenicia.Auth.Services;

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
