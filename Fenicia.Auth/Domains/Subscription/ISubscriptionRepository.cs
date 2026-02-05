using Fenicia.Common.Data.Abstracts;
using Fenicia.Common.Data.Models.Auth;

namespace Fenicia.Auth.Domains.Subscription;

public interface ISubscriptionRepository : IBaseRepository<SubscriptionModel>
{
    Task<List<Guid>> GetValidSubscriptionAsync(Guid companyId, CancellationToken cancellationToken);
}
