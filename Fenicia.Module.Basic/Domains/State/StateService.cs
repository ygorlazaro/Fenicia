using Fenicia.Common.Database.Models.Basic;

namespace Fenicia.Module.Basic.Domains.State;

public class StateService : IStateService
{
    private readonly IStateRepository _stateRepository;

    public StateService(IStateRepository stateRepository)
    {
        _stateRepository = stateRepository;
    }

    public async Task<List<StateModel>> GetAllAsync()
    {
        return await _stateRepository.GetAllAsync();
    }
}
