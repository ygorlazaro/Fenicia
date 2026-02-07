using Fenicia.Common.Enums.Auth;

namespace Fenicia.Auth.Domains.SubscriptionCredit;

public interface ISubscriptionCreditService
{
    Task<List<ModuleType>> GetActiveModulesTypesAsync(Guid companyId, CancellationToken ct);
}
