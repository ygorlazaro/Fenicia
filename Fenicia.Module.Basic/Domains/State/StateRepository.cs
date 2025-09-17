namespace Fenicia.Module.Basic.Domains.State;

using Common.Database.Contexts;
using Fenicia.Common.Database.Models.Basic;

using Microsoft.EntityFrameworkCore;

public class StateRepository : IStateRepository
{
    private readonly BasicContext basicContext;

    public StateRepository(BasicContext basicContext)
    {
        this.basicContext = basicContext;
    }

    public async Task<List<StateModel>> GetAllAsync()
    {
        return await this.basicContext.States.OrderBy(s => s.Uf).ToListAsync();
    }
}
