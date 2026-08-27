using Fenicia.Auth.Domains.Module.Queries;
using Fenicia.Auth.Domains.Module.Responses;
using Fenicia.Common;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Enums.Auth;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Module.Handlers;

public class GetModulesHandler(DefaultContext db) : IRequestHandler<GetModulesQuery, Pagination<List<GetModuleResponse>>>
{

    public async Task<Pagination<List<GetModuleResponse>>> Handle(GetModulesQuery query, CancellationToken ct)
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

        var modules = await request.Skip((query.Page - 1) * query.PerPage).Take(query.PerPage).ToListAsync(ct);

        var total = await request.CountAsync(ct);

        return new Pagination<List<GetModuleResponse>>(modules, total, query.Page, query.PerPage);
    }
}
