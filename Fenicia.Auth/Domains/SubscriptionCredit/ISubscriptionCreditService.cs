namespace Fenicia.Auth.Domains.SubscriptionCredit;

using Common;
using Common.Enums;

public interface ISubscriptionCreditService
{
    Task<ApiResponse<List<ModuleType>>> GetActiveModulesTypesAsync(Guid companyId, CancellationToken cancellationToken);
}
