using Fenicia.Common.Data.Abstracts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums;

namespace Fenicia.Auth.Domains.SubscriptionCredit;

public interface ISubscriptionCreditRepository : IBaseRepository<SubscriptionCreditModel>
{
    Task<List<ModuleType>> GetValidModulesTypesAsync(List<Guid> subscriptions, CancellationToken cancellationToken = default);
}
