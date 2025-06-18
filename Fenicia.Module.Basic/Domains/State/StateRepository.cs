using Fenicia.Module.Basic.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.State;

public class StateRepository(BasicContext basicContext) : IStateRepository
{
    public async Task<List<StateModel>> GetAllAsync()
    {
        return await basicContext.States.OrderBy(s => s.Uf).ToListAsync();
    }
}
