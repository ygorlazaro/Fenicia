using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.State.GetAll;

public class GetAllStateHandler(BasicContext context)
{
    public async Task<List<StateModel>> Handle(GetAllStateQuery query, CancellationToken ct)
    {
        return await context.States.ToListAsync(ct);
    }
}
