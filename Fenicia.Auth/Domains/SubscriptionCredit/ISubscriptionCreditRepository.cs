using Fenicia.Common.Enums;

namespace Fenicia.Auth.Domains.SubscriptionCredit;

public interface ISubscriptionCreditRepository
{
    Task<List<ModuleType>> GetValidModulesTypesAsync(List<Guid> subscriptions);
}
