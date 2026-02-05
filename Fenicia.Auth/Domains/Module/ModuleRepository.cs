using Fenicia.Common.Database.Abstracts;
using Fenicia.Common.Database.Contexts;
using Fenicia.Common.Database.Models.Auth;
using Fenicia.Common.Enums;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Module;

public class ModuleRepository(AuthContext context) : BaseRepository<ModuleModel>(context), IModuleRepository
{
    public override async Task<List<ModuleModel>> GetAllAsync(CancellationToken cancellationToken, int page = 1, int perPage = 10)
    {
        return await context.Modules
            .Where(m => m.Type != ModuleType.Erp && m.Type != ModuleType.Auth)
            .OrderBy(m => m.Type).Skip((page - 1) * perPage).Take(perPage).ToListAsync(cancellationToken);
    }

    public async Task<List<ModuleModel>> GetManyOrdersAsync(IEnumerable<Guid> request, CancellationToken cancellationToken)
    {
        return await context.Modules.Where(module => request.Any(r => r == module.Id)).OrderBy(module => module.Type).ToListAsync(cancellationToken);
    }

    public async Task<ModuleModel?> GetModuleByTypeAsync(ModuleType moduleType, CancellationToken cancellationToken)
    {
        return await context.Modules.FirstOrDefaultAsync(m => m.Type == moduleType, cancellationToken);
    }

    public async Task<List<ModuleModel>> LoadModulesAtDatabaseAsync(List<ModuleModel> modules, CancellationToken cancellationToken)
    {
        await context.Modules.AddRangeAsync(modules, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return await context.Modules.OrderBy(m => m.Type).ToListAsync(cancellationToken);
    }

    public async Task<List<ModuleModel>> GetUserModulesAsync(Guid userId, Guid companyId, CancellationToken cancellationToken)
    {
        var query = ValidModuleBySubscriptionQuery(userId, companyId);

        return await query.Distinct().Distinct().ToListAsync(cancellationToken);
    }

    public async Task<List<ModuleModel>> GetModuleAndSubmoduleAsync(Guid userId, Guid companyId, CancellationToken cancellationToken)
    {
        var query = ValidModuleBySubscriptionQuery(userId, companyId);

        return await query.Include(m => m.Submodules).Distinct().ToListAsync(cancellationToken);
    }

    private IQueryable<ModuleModel> ValidModuleBySubscriptionQuery(Guid userId, Guid companyId)
    {
        var now = DateTime.Now;

        return from module in context.Modules
               join subscriptionCredit in context.SubscriptionCredits on module.Id equals subscriptionCredit.ModuleId
               join subscription in context.Subscriptions on subscriptionCredit.SubscriptionId equals subscription.Id
               join userRole in context.UserRoles on subscription.CompanyId equals userRole.CompanyId
               where userRole.UserId == userId
               && subscription.CompanyId == companyId
               && subscription.Status == SubscriptionStatus.Active
               && now >= subscription.StartDate && now <= subscription.EndDate
               && subscriptionCredit.IsActive
               && now >= subscriptionCredit.StartDate && now <= subscriptionCredit.EndDate
               select module;
    }
}
