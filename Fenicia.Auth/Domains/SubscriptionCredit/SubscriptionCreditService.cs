using Fenicia.Auth.Domains.Subscription;
using Fenicia.Common;
using Fenicia.Common.Enums;

namespace Fenicia.Auth.Domains.SubscriptionCredit;

public class SubscriptionCreditService(ISubscriptionCreditRepository subscriptionCreditRepository, ISubscriptionService subscriptionService) : ISubscriptionCreditService
{
    public async Task<ApiResponse<List<ModuleType>>> GetActiveModulesTypesAsync(Guid companyId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var validSubscriptions = await subscriptionService.GetValidSubscriptionsAsync(companyId, cancellationToken);

        if (validSubscriptions.Data is null)
        {
            return new ApiResponse<List<ModuleType>>(data: null, validSubscriptions.Status, validSubscriptions.Message?.Message ?? string.Empty);
        }

        var validModules = await subscriptionCreditRepository.GetValidModulesTypesAsync(validSubscriptions.Data, cancellationToken);

        return new ApiResponse<List<ModuleType>>(validModules);
    }
}
