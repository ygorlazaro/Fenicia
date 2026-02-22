using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Enums.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.User.GetUserModules;

public class GetUserModuleHandler(AuthContext context)
{
    public async Task<List<ModuleResponse>> Handler(GetUserModulesQuery query, CancellationToken ct)
    {
        var request = ValidModuleBySubscriptionQuery(query.UserId, query.CompanyId);

        return await request.Distinct().ToListAsync(ct);
    }
    
    private IQueryable<ModuleResponse> ValidModuleBySubscriptionQuery(Guid userId, Guid companyId)
    {
        var now = DateTime.Now;

        var query = from m in context.Modules
                    join sc in context.SubscriptionCredits on m.Id equals sc.ModuleId
                    join s in context.Subscriptions on sc.SubscriptionId equals s.Id
                    join ur in context.UserRoles on s.CompanyId equals ur.CompanyId
                    where ur.UserId == userId
                          && s.CompanyId == companyId
                          && s.Status == SubscriptionStatus.Active
                          && now >= s.StartDate && now <= s.EndDate
                          && sc.IsActive
                          && now >= sc.StartDate && now <= sc.EndDate
                    select new ModuleResponse(m.Id, m.Name, m.Type);

        return query;
    }
}