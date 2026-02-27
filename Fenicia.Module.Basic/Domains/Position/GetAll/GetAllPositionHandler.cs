using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Position.GetAll;

public class GetAllPositionHandler(DefaultContext context)
{
    public async Task<List<GetAllPositionResponse>> Handle(GetAllPositionQuery query, CancellationToken ct)
    {
        return await context.BasicPositions
            .Select(p => new GetAllPositionResponse(p.Id, p.Name))
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(ct);
    }
}
