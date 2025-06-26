using Fenicia.Common;
using Fenicia.Common.Enums;

namespace Fenicia.Auth.Domains.SubscriptionCredit.Logic;

public interface ISubscriptionCreditService
{
    Task<ApiResponse<List<ModuleType>>> GetActiveModulesTypesAsync(Guid companyId,
        CancellationToken cancellationToken);
}
