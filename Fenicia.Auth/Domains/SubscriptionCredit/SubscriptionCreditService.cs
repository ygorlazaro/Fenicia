using Fenicia.Auth.Domains.Subscription;
using Fenicia.Common.Enums;

namespace Fenicia.Auth.Domains.SubscriptionCredit;

public class SubscriptionCreditService(ISubscriptionCreditRepository subscriptionCreditRepository, ISubscriptionService subscriptionService) : ISubscriptionCreditService
{
    public async Task<List<ModuleType>> GetActiveModulesTypesAsync(Guid companyId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var validSubscriptions = await subscriptionService.GetValidSubscriptionsAsync(companyId, cancellationToken);

        if (validSubscriptions.Count == 0)
        {
            return [];
        }

        return await subscriptionCreditRepository.GetValidModulesTypesAsync(validSubscriptions, cancellationToken);
    }
}
