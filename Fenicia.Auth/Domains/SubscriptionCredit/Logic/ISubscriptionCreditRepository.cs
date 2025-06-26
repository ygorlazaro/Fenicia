using Fenicia.Common.Enums;

namespace Fenicia.Auth.Domains.SubscriptionCredit.Logic;

public interface ISubscriptionCreditRepository
{
    Task<List<ModuleType>> GetValidModulesTypesAsync(List<Guid> subscriptions,
        CancellationToken cancellationToken);
}
