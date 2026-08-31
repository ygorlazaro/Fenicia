using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.State;

public class StateRepository(DefaultContext context) : Repository<StateModel>(context), IStateRepository
{
    public async Task<List<StateModel>> GetAllOrderedAsync(CancellationToken ct)
    {
        return await DbSet
                .OrderBy(s => s.Uf)
            .ToListAsync(ct);
    }
}
