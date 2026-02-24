using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Position.GetById;

public class GetPositionByIdHandler(BasicContext context)
{
    public async Task<GetPositionByIdResponse?> Handle(GetPositionByIdQuery query, CancellationToken ct)
    {
        var position = await context.Positions.FirstOrDefaultAsync(p => p.Id == query.Id, ct);

        return position is null ? null : new GetPositionByIdResponse(position.Id, position.Name);
    }
}