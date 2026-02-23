using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Position.GetAll;

public class GetAllPositionHandler(BasicContext context)
{
    public async Task<List<PositionResponse>> Handle(GetAllPositionQuery query, CancellationToken ct)
    {
        var positions = await context.Positions
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(ct);

        return positions.Select(p => new PositionResponse(p.Id, p.Name)).ToList();
    }
}
