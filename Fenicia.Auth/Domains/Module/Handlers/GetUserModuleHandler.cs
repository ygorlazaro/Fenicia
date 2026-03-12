using Fenicia.Auth.Domains.Module.Queries;
using Fenicia.Auth.Domains.Module.Responses;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Enums.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Module.Handlers;

public class GetUserModuleHandler(DefaultContext db)
{
    public async Task<List<GetUserModulesResponse>> Handle(GetUserModulesQuery query, CancellationToken ct)
    {
        var request = ValidModuleBySubscriptionQuery(query.UserId,
            query.CompanyId);

        return await request.Distinct().ToListAsync(ct);
    }

    private IQueryable<GetUserModulesResponse> ValidModuleBySubscriptionQuery(Guid userId, Guid companyId)
    {
        var now = DateTime.Now;

        var query = from m in db.AuthModules
                    join sc in db.AuthSubscriptionCredits on m.Id equals sc.ModuleId
                    join s in db.AuthSubscriptions on sc.SubscriptionId equals s.Id
                    join ur in db.AuthUserRoles on s.CompanyId equals ur.CompanyId
                    where ur.UserId == userId
                          && s.CompanyId == companyId
                          && s.Status == SubscriptionStatus.Active
                          && now >= s.StartDate && now <= s.EndDate
                          && sc.IsActive
                          && now >= sc.StartDate && now <= sc.EndDate
                    select new GetUserModulesResponse(m.Id,
                        m.Name,
                        m.Type);

        return query;
    }
}
