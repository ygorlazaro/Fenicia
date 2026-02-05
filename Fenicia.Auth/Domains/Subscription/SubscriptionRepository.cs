using Fenicia.Common.Data.Abstracts;
using Fenicia.Common.Data.Contexts;

namespace Fenicia.Auth.Domains.Subscription;

using Common.Enums;

using Fenicia.Common.Data.Models.Auth;

using Microsoft.EntityFrameworkCore;

public class SubscriptionRepository(AuthContext context) : BaseRepository<SubscriptionModel>(context), ISubscriptionRepository
{
    public async Task<List<Guid>> GetValidSubscriptionAsync(Guid companyId, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var subscriptions = context.Subscriptions.Where(subscription => subscription.CompanyId == companyId && now >= subscription.StartDate && now <= subscription.EndDate && subscription.Status == SubscriptionStatus.Active).Select(subscription => subscription.Id);

        return await subscriptions.ToListAsync(cancellationToken);
    }
}
