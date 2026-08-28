using Fenicia.Common.Data.Models.Auth;
using Fenicia.Module.Basic.Domains.State.DTOs;
using Fenicia.Module.Basic.Domains.State;

namespace Fenicia.Module.Basic.Domains.State;

public class StateService(StateRepository stateRepository)
{
    public async Task<List<GetAllStateResponse>> GetAllAsync(CancellationToken ct)
    {
        var states = await stateRepository.GetAllOrderedAsync(ct);

        return states.Select(s => new GetAllStateResponse(s.Id, s.Name, s.Uf)).ToList();
    }
}
