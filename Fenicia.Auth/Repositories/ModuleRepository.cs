using Fenicia.Auth.Contexts;
using Fenicia.Auth.Contexts.Models;
using Fenicia.Auth.Enums;
using Fenicia.Auth.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Repositories;

public class ModuleRepository(AuthContext authContext) : IModuleRepository
{
    public async Task<List<ModuleModel>> GetAllOrderedAsync()
    {
        return await authContext.Modules.OrderBy(m => m.Type).ToListAsync();
    }

    public async Task<List<ModuleModel?>> GetManyOrdersAsync(IEnumerable<Guid> request)
    {
        var query = from module in authContext.Modules
                    where request.Any(r => r == module.Id)
                    orderby module.Type
                    select module;

        return await query.ToListAsync();
    }

    public async Task<ModuleModel?> GetModuleByTypeAsync(ModuleType moduleType)
    {
        return await authContext.Modules.FirstOrDefaultAsync(m => m.Type == moduleType);
    }
}