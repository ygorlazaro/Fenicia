using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.State.Responses;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.State.Handlers;

public class GetAllStateHandler(DefaultContext db)
{
    public async Task<List<GetAllStateResponse>> Handle(CancellationToken ct)
    {
        return await db.AuthStates
            .OrderBy(s => s.Uf)
            .Select(s => new GetAllStateResponse(s.Id,
                s.Name,
                s.Uf))
            .ToListAsync(ct);
    }
}
