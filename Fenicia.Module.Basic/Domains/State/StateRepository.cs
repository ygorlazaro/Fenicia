using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;
using Fenicia.Module.Basic.Domains.State.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.State;

public class StateRepository(DbContext context) : Repository<StateModel>(context), IStateRepository
{
    public Task<List<StateModel>> GetAllOrderedAsync(CancellationToken cancellationToken = default)
    {
        return DbSet
            .OrderBy(s => s.Uf)
            .ToListAsync(cancellationToken);
    }
}