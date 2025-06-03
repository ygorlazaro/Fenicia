using Fenicia.Auth.Repositories.Interfaces;
using Fenicia.Auth.Services.Interfaces;
using Fenicia.Common.Enums;

namespace Fenicia.Auth.Services;

public class SubscriptionCreditService(ILogger<SubscriptionCreditService> logger, ISubscriptionCreditRepository subscriptionCreditRepository, ISubscriptionService subscriptionService): ISubscriptionCreditService
{
    public async Task<List<ModuleType>> GetActiveModulesTypesAsync(Guid companyId)
    {
        logger.LogInformation("Getting active modules types");
        
        var validSubscriptions = await subscriptionService.GetValidSubscriptionsAsync(companyId);
        var validModules = await subscriptionCreditRepository.GetValidModulesTypesAsync(validSubscriptions);
        
        return validModules;
    }
}