using Fenicia.Auth.Contexts;
using Fenicia.Auth.Domains.Module.Data;
using Fenicia.Common.Enums;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Module.Logic;

public class ModuleRepository(AuthContext authContext) : IModuleRepository
{
    public async Task<List<ModuleModel>> GetAllOrderedAsync(CancellationToken cancellationToken,
        int page = 1, int perPage = 10)
    {
        return await authContext
            .Modules.OrderBy(m => m.Type)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<ModuleModel>> GetManyOrdersAsync(IEnumerable<Guid> request,
        CancellationToken cancellationToken)
    {
        var query =
            from module in authContext.Modules
            where request.Any(r => r == module.Id)
            orderby module.Type
            select module;

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<ModuleModel?> GetModuleByTypeAsync(ModuleType moduleType,
        CancellationToken cancellationToken)
    {
        return await authContext.Modules.FirstOrDefaultAsync(m => m.Type == moduleType, cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken)
    {
        return await authContext.Modules.CountAsync(cancellationToken);
    }
}
