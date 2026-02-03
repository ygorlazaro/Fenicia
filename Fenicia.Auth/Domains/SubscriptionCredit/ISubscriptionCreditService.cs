using Fenicia.Common.Enums;

namespace Fenicia.Auth.Domains.SubscriptionCredit;

public interface ISubscriptionCreditService
{
    Task<List<ModuleType>> GetActiveModulesTypesAsync(Guid companyId, CancellationToken cancellationToken);
}
