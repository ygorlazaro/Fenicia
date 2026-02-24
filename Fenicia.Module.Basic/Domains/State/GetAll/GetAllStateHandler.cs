using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.State.GetAll;

public class GetAllStateHandler(BasicContext context)
{
    public async Task<List<GetAllStateResponse>> Handle(GetAllStateQuery query, CancellationToken ct)
    {
        return await context.States
            .Select(s => new GetAllStateResponse(s.Id, s.Name, s.Uf))
            .ToListAsync(ct);
    }
}