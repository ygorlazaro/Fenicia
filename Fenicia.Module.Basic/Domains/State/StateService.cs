using Fenicia.Common.DTOs.Basic.State;
using Fenicia.Module.Basic.Domains.State.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.State;

public sealed class StateService(IStateRepository stateRepository, StateMapper stateMapper) : IStateService
{
    public StateService()
        : this(null!, null!)
    {
    }

    public async Task<List<GetAllStateResponse>> GetAllAsync(
        GetAllStateQuery query,
        CancellationToken cancellationToken = default)
    {
        var baseQuery = stateRepository.Query();

        var states = await baseQuery
            .OrderBy(s => s.Uf)
            .ToListAsync(cancellationToken);

        return [.. states.Select(stateMapper.MapToGetAllStateResponse)];
    }
}