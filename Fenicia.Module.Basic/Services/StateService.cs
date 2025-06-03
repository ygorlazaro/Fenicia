using Fenicia.Module.Basic.Contexts.Models;
using Fenicia.Module.Basic.Repositories.Interfaces;
using Fenicia.Module.Basic.Services.Interfaces;

namespace Fenicia.Module.Basic.Services;

public class StateService(IStateRepository stateRepository) : IStateService
{
    public async Task<List<StateModel>> GetAllAsync()
    {
        return await stateRepository.GetAllAsync();
    }
}