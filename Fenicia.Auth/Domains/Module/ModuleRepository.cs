using Fenicia.Common.Data.Abstracts;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Module;

public class ModuleRepository(AuthContext context) : BaseRepository<ModuleModel>(context), IModuleRepository
{
    public override async Task<List<ModuleModel>> GetAllAsync(CancellationToken ct, int page = 1, int perPage = 10)
    {
        return await context.Modules
            .Where(m => m.Type != ModuleType.Erp && m.Type != ModuleType.Auth)
            .OrderBy(m => m.Type)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync(ct);
    }

    public async Task<List<ModuleModel>> GetManyOrdersAsync(IEnumerable<Guid> request, CancellationToken ct)
    {
        return await context.Modules.Where(module => request.Any(r => r == module.Id))
                                    .OrderBy(module => module.Type)
                                    .ToListAsync(ct);
    }

    public async Task<ModuleModel?> GetModuleByTypeAsync(ModuleType moduleType, CancellationToken ct)
    {
        return await context.Modules.FirstOrDefaultAsync(m => m.Type == moduleType, ct);
    }

    public async Task<List<ModuleModel>> LoadModulesAtDatabaseAsync(List<ModuleModel> modules, CancellationToken ct)
    {
        await context.Modules.AddRangeAsync(modules, ct);
        await context.SaveChangesAsync(ct);

        return await context.Modules.OrderBy(m => m.Type).ToListAsync(ct);
    }

    public async Task<List<ModuleModel>> GetUserModulesAsync(Guid userId, Guid companyId, CancellationToken ct)
    {
        var query = ValidModuleBySubscriptionQuery(userId, companyId);

        return await query.Distinct().Distinct().ToListAsync(ct);
    }

    public async Task<List<ModuleModel>> GetModuleAndSubmoduleAsync(Guid userId, Guid companyId, CancellationToken ct)
    {
        var query = ValidModuleBySubscriptionQuery(userId, companyId);

        return await query.Include(m => m.Submodules).Distinct().ToListAsync(ct);
    }

    private IQueryable<ModuleModel> ValidModuleBySubscriptionQuery(Guid userId, Guid companyId)
    {
        var now = DateTime.Now;

        return from m in context.Modules
               join sc in context.SubscriptionCredits on m.Id equals sc.ModuleId
               join s in context.Subscriptions on sc.SubscriptionId equals s.Id
               join ur in context.UserRoles on s.CompanyId equals ur.CompanyId
               where ur.UserId == userId
                     && s.CompanyId == companyId
                     && s.Status == SubscriptionStatus.Active
                     && now >= s.StartDate && now <= s.EndDate
                     && sc.IsActive
                     && now >= sc.StartDate && now <= sc.EndDate
               select m;
    }
}
