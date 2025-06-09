using Fenicia.Module.Basic.Contexts.Models;

namespace Fenicia.Module.Basic.Repositories.Interfaces;

public interface IStateRepository
{
    Task<List<StateModel>> GetAllAsync();
}
