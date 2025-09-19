namespace Fenicia.Module.Basic.Domains.State;

using Fenicia.Common.Database.Models.Basic;

public class StateService : IStateService
{
    private readonly IStateRepository _stateRepository;

    public StateService(IStateRepository stateRepository)
    {
        this._stateRepository = stateRepository;
    }

    public async Task<List<StateModel>> GetAllAsync()
    {
        return await _stateRepository.GetAllAsync();
    }
}
