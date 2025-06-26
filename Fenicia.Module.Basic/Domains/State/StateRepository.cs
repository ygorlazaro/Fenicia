namespace Fenicia.Module.Basic.Domains.State;

using Contexts;

using Microsoft.EntityFrameworkCore;

public class StateRepository(BasicContext basicContext) : IStateRepository
{
    public async Task<List<StateModel>> GetAllAsync()
    {
        return await basicContext.States.OrderBy(s => s.Uf).ToListAsync();
    }
}
