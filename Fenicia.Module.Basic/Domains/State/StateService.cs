namespace Fenicia.Module.Basic.Domains.State;

using Fenicia.Common.Database.Models.Basic;

public class StateService : IStateService
{
    private readonly IStateRepository stateRepository;

    public StateService(IStateRepository stateRepository)
    {
        this.stateRepository = stateRepository;
    }

    public async Task<List<StateModel>> GetAllAsync()
    {
        return await this.stateRepository.GetAllAsync();
    }
}
