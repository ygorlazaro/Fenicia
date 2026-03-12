using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.DataSource.Responses;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.DataSource.Handlers;

public class GetAllProductForDataSourceHandler(DefaultContext db)
{
    public async Task<List<GetAllProductForDataSourceResponse>> Handle(CancellationToken ct)
    {
        return await db.BasicProducts
            .OrderBy(p => p.Name)
            .Select(p => new GetAllProductForDataSourceResponse(p.Id,
                p.Name))
            .ToListAsync(ct);
    }
}