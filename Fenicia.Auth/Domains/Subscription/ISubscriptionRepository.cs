using Fenicia.Common.Database.Abstracts;
using Fenicia.Common.Database.Models.Auth;

namespace Fenicia.Auth.Domains.Subscription;

public interface ISubscriptionRepository : IBaseRepository<SubscriptionModel>
{
    Task<List<Guid>> GetValidSubscriptionAsync(Guid companyId, CancellationToken cancellationToken);
}
