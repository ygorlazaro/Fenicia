using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.State.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.State;

public class StateService(DefaultContext db)
{
    public async Task<List<GetAllStateResponse>> GetAllAsync(CancellationToken ct)
    {
        return await db.AuthStates.OrderBy(s => s.Uf).Select(s => new GetAllStateResponse(s.Id, s.Name, s.Uf)).ToListAsync(ct);
    }
}
