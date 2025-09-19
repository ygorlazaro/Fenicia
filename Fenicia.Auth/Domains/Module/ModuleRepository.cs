namespace Fenicia.Auth.Domains.Module;

using Common.Enums;

using Common.Database.Contexts;
using Fenicia.Common.Database.Models.Auth;

using Microsoft.EntityFrameworkCore;

public class ModuleRepository : IModuleRepository
{
    private readonly AuthContext _authContext;

    public ModuleRepository(AuthContext authContext)
    {
        this._authContext = authContext;
    }

    public async Task<List<ModuleModel>> GetAllOrderedAsync(CancellationToken cancellationToken, int page = 1, int perPage = 10)
    {
        return await _authContext.Modules.OrderBy(m => m.Type).Skip((page - 1) * perPage).Take(perPage).ToListAsync(cancellationToken);
    }

    public async Task<List<ModuleModel>> GetManyOrdersAsync(IEnumerable<Guid> request, CancellationToken cancellationToken)
    {
        var query = from module in _authContext.Modules where request.Any(r => r == module.Id) orderby module.Type select module;

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<ModuleModel?> GetModuleByTypeAsync(ModuleType moduleType, CancellationToken cancellationToken)
    {
        return await _authContext.Modules.FirstOrDefaultAsync(m => m.Type == moduleType, cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken)
    {
        return await _authContext.Modules.CountAsync(cancellationToken);
    }

    public async Task<List<ModuleModel>> LoadModulesAtDatabaseAsync(List<ModuleModel> modules, CancellationToken cancellationToken)
    {
        await _authContext.Modules.AddRangeAsync(modules, cancellationToken);
        await _authContext.SaveChangesAsync(cancellationToken);
        return await _authContext.Modules.OrderBy(m => m.Type).ToListAsync(cancellationToken);
    }

    public async Task<List<ModuleModel>> GetUserModulesAsync(Guid userId, Guid companyId, CancellationToken cancellationToken)
    {
        var query = ValidModuleBySubscriptionQuery(userId, companyId);

        return await query.Distinct().ToListAsync(cancellationToken);
    }

    public async Task<List<ModuleModel>> GetModuleAndSubmoduleAsync(Guid userId, Guid companyId, CancellationToken cancellationToken)
    {
        var query = ValidModuleBySubscriptionQuery(userId, companyId);

        return await query.Include(m => m.Submodules).ToListAsync(cancellationToken);
    }

    private IQueryable<ModuleModel> ValidModuleBySubscriptionQuery(Guid userId, Guid companyId)
    {
        var now = DateTime.Now;

        return from module in _authContext.Modules
               join subscriptionCredit in _authContext.SubscriptionCredits on module.Id equals subscriptionCredit.ModuleId
               join subscription in _authContext.Subscriptions on subscriptionCredit.SubscriptionId equals subscription.Id
               join userRole in _authContext.UserRoles on subscription.CompanyId equals userRole.CompanyId
               where userRole.UserId == userId
               && subscription.CompanyId == companyId
               && subscription.Status == SubscriptionStatus.Active
               && now >= subscription.StartDate && now <= subscription.EndDate
               && subscriptionCredit.IsActive
               && now >= subscriptionCredit.StartDate && now <= subscriptionCredit.EndDate
               select module;
    }
}
