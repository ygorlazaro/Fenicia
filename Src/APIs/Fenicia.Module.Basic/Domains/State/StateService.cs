using Fenicia.Common;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.State.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.State;

public class StateService(IStateRepository stateRepository)
{
    public async Task<List<GetAllStateResponse>> GetAllAsync(GetAllStateQuery query, CancellationToken ct)
    {
        var states = await stateRepository.GetAllOrderedAsync(ct);

        return [.. states.Select(s => s.MapToGetAllStateResponse())];
    }
}
