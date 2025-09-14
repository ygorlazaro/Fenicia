namespace Fenicia.Module.Basic.Domains.State;

using Common.Database.Contexts;
using Fenicia.Common.Database.Models.Basic;

using Microsoft.EntityFrameworkCore;

public class StateRepository : IStateRepository
{
    private readonly BasicContext _basicContext;

    public StateRepository(BasicContext basicContext)
    {
        _basicContext = basicContext;
    }

    public async Task<List<StateModel>> GetAllAsync()
    {
        return await _basicContext.States.OrderBy(s => s.Uf).ToListAsync();
    }
}
