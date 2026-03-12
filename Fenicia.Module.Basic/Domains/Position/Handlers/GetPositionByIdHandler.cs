using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Position.Queries;
using Fenicia.Module.Basic.Domains.Position.Responses;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Position.Handlers;

public class GetPositionByIdHandler(DefaultContext db)
{
    public async Task<GetPositionByIdResponse?> Handle(GetPositionByIdQuery query, CancellationToken ct)
    {
        var position = await db.BasicPositions.FirstOrDefaultAsync(p => p.Id == query.Id,
            ct);

        return position is null ? null : new GetPositionByIdResponse(position.Id,
            position.Name);
    }
}
