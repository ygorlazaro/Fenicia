using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.State.Queries;
using Fenicia.Module.Basic.Domains.State.Responses;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.State.Handlers;

public class GetAllStateHandler(DefaultContext db) : IRequestHandler<GetAllStateQuery, List<GetAllStateResponse>>
{

    public async Task<List<GetAllStateResponse>> Handle(GetAllStateQuery query, CancellationToken ct)
    {
        return await db.AuthStates.OrderBy(s => s.Uf).Select(s => new GetAllStateResponse(s.Id, s.Name, s.Uf)).ToListAsync(ct);
    }
}
