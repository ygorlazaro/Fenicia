using Fenicia.Auth.Contexts;
using Fenicia.Common.Enums;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.SubscriptionCredit;

public class SubscriptionCreditRepository(AuthContext authContext) : ISubscriptionCreditRepository
{
    public async Task<List<ModuleType>> GetValidModulesTypesAsync(List<Guid> subscriptions)
    {
        var now = DateTime.UtcNow;

        var query =
            from credit in authContext.SubscriptionCredits
            join module in authContext.Modules on credit.ModuleId equals module.Id
            where
                credit.IsActive
                && subscriptions.Contains(credit.SubscriptionId)
                && now >= credit.StartDate
                && now <= credit.EndDate
            orderby module.Id
            select module.Type;

        return await query.Distinct().ToListAsync();
    }
}
