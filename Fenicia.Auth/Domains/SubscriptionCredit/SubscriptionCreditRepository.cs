using Fenicia.Common.Data.Abstracts;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.SubscriptionCredit;

public class SubscriptionCreditRepository(AuthContext context)
    : BaseRepository<SubscriptionCreditModel>(context), ISubscriptionCreditRepository
{
    public async Task<List<ModuleType>> GetValidModulesTypesAsync(List<Guid> subscriptions, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var query = from c in context.SubscriptionCredits
                    join m in context.Modules on c.ModuleId equals m.Id
                    where c.IsActive
                          && subscriptions.Contains(c.SubscriptionId)
                          && now >= c.StartDate
                          && now <= c.EndDate
                    orderby m.Id
                    select m.Type;

        return await query.Distinct().ToListAsync(ct);
    }
}