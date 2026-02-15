using Fenicia.Common;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Enums.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Module.GetModules;

public class GetModulesHandler(AuthContext db)
{
    public async Task<Pagination<List<ModuleResponse>>> Handle(GetModulesRequest request, CancellationToken ct)
    {
        var query = db.Modules
            .Where(m => m.Type != ModuleType.Erp && m.Type != ModuleType.Auth)
            .OrderBy(m => m.Type)
            .Select(m => new ModuleResponse(m.Id, m.Name, m.Type));
        
        var modules = await query
            .Skip((request.Page  - 1) * request.PerPage)
            .Take(request.PerPage)
            .ToListAsync(ct);
        
        var total = await query.CountAsync(ct);

        return new Pagination<List<ModuleResponse>>(modules, total, request.Page, request.PerPage);
    }
}

public sealed record ModuleResponse (Guid Id, string Name, ModuleType Type); 
