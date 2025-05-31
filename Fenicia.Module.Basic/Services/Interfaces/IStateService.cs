using Fenicia.Module.Basic.Contexts.Models;

namespace Fenicia.Module.Basic.Services.Interfaces;

public interface IStateService
{
    Task<List<StateModel>> GetAllAsync();
}