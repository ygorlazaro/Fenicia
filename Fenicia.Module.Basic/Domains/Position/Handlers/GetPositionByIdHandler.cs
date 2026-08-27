using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Position.DTOs.Queries;
using Fenicia.Module.Basic.Domains.Position.DTOs.Responses;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Position.Handlers;

public class GetPositionByIdHandler(DefaultContext db) : IRequestHandler<GetPositionByIdQuery, GetPositionByIdResponse?>
{

    public async Task<GetPositionByIdResponse?> Handle(GetPositionByIdQuery query, CancellationToken ct)
    {
        var position = await db.BasicPositions.FirstOrDefaultAsync(p => p.Id == query.Id, ct);

        return position is null ? null : new GetPositionByIdResponse(position.Id, position.Name);
    }
}
