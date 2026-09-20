using Fenicia.Common.DTOs.Basic.State;
using Fenicia.Module.Basic.Domains.State.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.State;

public sealed class StateService(IStateRepository repository) : IStateService
{
    public StateService()
        : this(null!)
    {
    }

    public async Task<List<GetAllStateResponse>> GetAllAsync(
        GetAllStateQuery query,
        CancellationToken cancellationToken = default)
    {
        var baseQuery = repository.Query();

        var states = await baseQuery
            .OrderBy(s => s.Uf)
            .ToListAsync(cancellationToken);

        return [.. states.Select(s => new GetAllStateResponse(s.Id, s.Name, s.Uf))];
    }
}