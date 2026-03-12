using Fenicia.Common;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Position.Queries;
using Fenicia.Module.Basic.Domains.Position.Responses;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Position.Handlers;

public class GetAllPositionHandler(DefaultContext db)
{
    public async Task<Pagination<List<GetAllPositionResponse>>> Handle(GetAllPositionQuery query, CancellationToken ct)
    {
        var total = await db.BasicPositions.CountAsync(ct);
    
        var positions = await db.BasicPositions
            .Select(p => new GetAllPositionResponse(p.Id,
                p.Name))
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(ct);

        return new Pagination<List<GetAllPositionResponse>>(positions,
            total,
            query.Page,
            query.PerPage);
    }
}
