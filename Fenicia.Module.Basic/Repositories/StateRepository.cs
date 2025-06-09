using Fenicia.Module.Basic.Contexts;
using Fenicia.Module.Basic.Contexts.Models;
using Fenicia.Module.Basic.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Repositories;

public class StateRepository(BasicContext basicContext) : IStateRepository
{
    public async Task<List<StateModel>> GetAllAsync()
    {
        return await basicContext.States.OrderBy(s => s.Uf).ToListAsync();
    }
}
