using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.DataSource.Responses;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.DataSource.Handlers;

public class GetAllPositionForDataSourceHandler(DefaultContext db)
{
    public async Task<List<GetAllPositionForDataSourceResponse>> Handle(CancellationToken ct)
    {
        return await db.BasicPositions
            .OrderBy(p => p.Name)
            .Select(p => new GetAllPositionForDataSourceResponse(p.Id,
                p.Name))
            .ToListAsync(ct);
    }
}