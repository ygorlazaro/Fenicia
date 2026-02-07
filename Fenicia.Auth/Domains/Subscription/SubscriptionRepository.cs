using Fenicia.Common.Data.Abstracts;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Subscription;

public class SubscriptionRepository(AuthContext context) : BaseRepository<SubscriptionModel>(context), ISubscriptionRepository
{
    public async Task<List<Guid>> GetValidSubscriptionAsync(Guid companyId, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var subscriptions = context.Subscriptions
            .Where(s => s.CompanyId == companyId && now >= s.StartDate && now <= s.EndDate && s.Status == SubscriptionStatus.Active)
            .Select(s => s.Id);

        return await subscriptions.ToListAsync(ct);
    }
}
