namespace Fenicia.Auth.Domains.SubscriptionCredit.Logic;

using Common.Enums;

public interface ISubscriptionCreditRepository
{
    Task<List<ModuleType>> GetValidModulesTypesAsync(List<Guid> subscriptions, CancellationToken cancellationToken = default);
}
