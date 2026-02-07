using Fenicia.Common.Data.Models.Basic;

namespace Fenicia.Module.Basic.Domains.State;

public class StateService(IStateRepository stateRepository) : IStateService
{
    public async Task<List<StateModel>> GetAllAsync(CancellationToken ct)
    {
        return await stateRepository.GetAllAsync(ct);
    }
}
