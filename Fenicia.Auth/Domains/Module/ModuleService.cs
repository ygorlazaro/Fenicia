using Fenicia.Auth.Domains.Module.DTOs.Responses;
using Fenicia.Common;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Enums.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Module;

public class ModuleService(DefaultContext db)
{
    public async Task<Pagination<List<GetModuleResponse>>> GetAllModulesAsync(int page, int perPage, CancellationToken ct)
    {
        var request = db.AuthModules.Where(m => m.Type != ModuleType.Auth && m.IsActive)
            .OrderBy(m => m.SortOrder)
            .Select(m => new GetModuleResponse(m.Id,
                m.Name,
                m.Type,
                m.Description,
                m.Icon,
                m.IsActive,
                m.SortOrder,
                m.Price));

        var modules = await request.Skip((page - 1) * perPage).Take(perPage).ToListAsync(ct);

        var total = await request.CountAsync(ct);

        return new Pagination<List<GetModuleResponse>>(modules, total, page, perPage);
    }

    public async Task<List<GetUserModulesResponse>> GetUserModulesAsync(Guid companyId, Guid userId, CancellationToken ct)
    {
        var now = DateTime.Now;

        var query = from m in db.AuthModules
                    join sc in db.AuthSubscriptionCredits on m.Id equals sc.ModuleId
                    join s in db.AuthSubscriptions on sc.SubscriptionId equals s.Id
                    join ur in db.AuthUserRoles on s.CompanyId equals ur.CompanyId
                    where ur.UserId == userId &&
                          s.CompanyId == companyId &&
                          s.Status == SubscriptionStatus.Active &&
                          now >= s.StartDate &&
                          now <= s.EndDate &&
                          sc.IsActive &&
                          now >= sc.StartDate &&
                          now <= sc.EndDate
                    select new GetUserModulesResponse(m.Id,
                        m.Name,
                        m.Type);

        return await query.Distinct().ToListAsync(ct);
    }
}
