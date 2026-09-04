using Fenicia.Common;
using Fenicia.Module.Basic.Domains.State.DTOs;
using Fenicia.Module.Basic.Domains.State.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.State;

public sealed class StateService(IStateRepository stateRepository) : IStateService
{
    public StateService()
        : this(null!)
    {
    }

    public async Task<List<GetAllStateResponse>> GetAllAsync(
        GetAllStateQuery query,
        CancellationToken cancellationToken = default)
    {
        var baseQuery = stateRepository.Query();

        var filteredQuery = baseQuery;

        var states = await filteredQuery
            .OrderBy(s => s.Uf)
            .ToListAsync(cancellationToken);

        return [.. states.Select(s => s.MapToGetAllStateResponse())];
    }
}