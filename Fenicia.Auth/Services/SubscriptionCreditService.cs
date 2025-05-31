using Fenicia.Auth.Enums;
using Fenicia.Auth.Repositories.Interfaces;
using Fenicia.Auth.Services.Interfaces;

namespace Fenicia.Auth.Services;

public class SubscriptionCreditService(ISubscriptionCreditRepository subscriptionCreditRepository, ISubscriptionService subscriptionService): ISubscriptionCreditService
{
    public async Task<List<ModuleType>> GetActiveModulesTypesAsync(Guid companyId)
    {
        var validSubscriptions = await subscriptionService.GetValidSubscriptionsAsync(companyId);
        var validModules = await subscriptionCreditRepository.GetValidModulesTypesAsync(validSubscriptions);
        
        return validModules;
    }
}