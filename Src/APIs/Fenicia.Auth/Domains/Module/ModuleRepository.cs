using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;
using Fenicia.Common.Enums.Auth;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Module;

public class ModuleRepository(DefaultContext context) : Repository<ModuleModel>(context)
{
    public async Task<List<ModuleModel>> GetAllActiveAsync(int page, int perPage, CancellationToken ct = default)
    {
        var query = from m in DbSet
                    where m.Type != ModuleType.Auth && m.IsActive
                    orderby m.SortOrder
                    select m;

        return await query.Skip((page - 1) * perPage).Take(perPage).ToListAsync(ct);
    }

    public async Task<int> CountAllActiveAsync(CancellationToken ct = default)
    {
        return await DbSet.CountAsync(m => m.Type != ModuleType.Auth && m.IsActive, ct);
    }

    public async Task<List<ModuleModel>> GetUserModulesAsync(Guid companyId, Guid userId, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;

        var query = from m in DbSet
                    join sc in Context.AuthSubscriptionCredits on m.Id equals sc.ModuleId
                    join s in Context.AuthSubscriptions on sc.SubscriptionId equals s.Id
                    join ur in Context.AuthUserRoles on s.CompanyId equals ur.CompanyId
                    where ur.UserId == userId &&
                          s.CompanyId == companyId &&
                          s.Status == SubscriptionStatus.Active &&
                          now >= s.StartDate && now <= s.EndDate &&
                          sc.IsActive &&
                          now >= sc.StartDate && now <= sc.EndDate
                    select m;

        return await query.Distinct().ToListAsync(ct);
    }

    public async Task<List<ModuleModel>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default)
    {
        return await DbSet.Where(m => ids.Contains(m.Id)).OrderBy(m => m.Type).ToListAsync(ct);
    }

    public async Task<ModuleModel?> GetByTypeAsync(ModuleType type, CancellationToken ct = default)
    {
        return await DbSet.FirstOrDefaultAsync(m => m.Type == type, ct);
    }
}
