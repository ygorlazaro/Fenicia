using Fenicia.Auth.Domains.Subscription;
using Fenicia.Common.Enums.Auth;

namespace Fenicia.Auth.Domains.SubscriptionCredit;

public class SubscriptionCreditService(ISubscriptionCreditRepository subscriptionCreditRepository, ISubscriptionService subscriptionService) : ISubscriptionCreditService
{
    public async Task<List<ModuleType>> GetActiveModulesTypesAsync(Guid companyId, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var validSubscriptions = await subscriptionService.GetValidSubscriptionsAsync(companyId, ct);

        return validSubscriptions.Count == 0 ? [] : await subscriptionCreditRepository.GetValidModulesTypesAsync(validSubscriptions, ct);
    }
}