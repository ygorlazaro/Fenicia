using Fenicia.Common.Database.Models.Basic;

namespace Fenicia.Module.Basic.Domains.State;

public class StateService(IStateRepository stateRepository) : IStateService
{
    public async Task<List<StateModel>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await stateRepository.GetAllAsync(cancellationToken);
    }
}
