using Fenicia.Common.Database.Abstracts;

namespace Fenicia.Auth.Domains.Subscription;

using Common.Database.Contexts;
using Common.Enums;

using Fenicia.Common.Database.Models.Auth;

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
