using Fenicia.Common;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Enums.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Module.GetModules;

public class GetModulesHandler(DefaultContext db)
{
    public async Task<Pagination<List<GetModuleResponse>>> Handle(GetModulesRequest query, CancellationToken ct)
    {
        var request = db.Modules
            .Where(m => m.Type != ModuleType.Auth)
            .OrderBy(m => m.Type)
            .Select(m => new GetModuleResponse(m.Id, m.Name, m.Type));

        var modules = await request
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(ct);

        var total = await request.CountAsync(ct);

        return new Pagination<List<GetModuleResponse>>(modules, total, query.Page, query.PerPage);
    }
}
