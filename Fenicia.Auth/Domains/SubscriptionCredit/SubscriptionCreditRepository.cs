using Fenicia.Common.Data.Abstracts;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.SubscriptionCredit;

public class SubscriptionCreditRepository(AuthContext context) : BaseRepository<SubscriptionCreditModel>(context), ISubscriptionCreditRepository
{
    public async Task<List<ModuleType>> GetValidModulesTypesAsync(List<Guid> subscriptions, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var query = from credit in context.SubscriptionCredits
                    join module in context.Modules on credit.ModuleId equals module.Id
                    where credit.IsActive
                          && subscriptions.Contains(credit.SubscriptionId)
                          && now >= credit.StartDate
                          && now <= credit.EndDate
                    orderby module.Id
                    select module.Type;

        return await query.Distinct().ToListAsync(cancellationToken);
    }
}
