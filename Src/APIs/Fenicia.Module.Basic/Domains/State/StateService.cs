using Fenicia.Common;
using Fenicia.Module.Basic.Domains.State.DTOs;
using Fenicia.Module.Basic.Domains.State.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.State;

public class StateService(IStateRepository stateRepository) : IStateService
{
    public StateService()
        : this(null!)
    {
    }

    public virtual async Task<List<GetAllStateResponse>> GetAllAsync(GetAllStateQuery query, CancellationToken cancellationToken = default)
    {
        var baseQuery = stateRepository.Query();

        var filters = AdvancedQueryParser.Parse(query.Query);
        var filteredQuery = baseQuery.ApplyAdvancedQuery(filters, query.Sort);

        var states = await filteredQuery
            .OrderBy(s => s.Uf)
            .ToListAsync(cancellationToken);

        return [.. states.Select(s => s.MapToGetAllStateResponse())];
    }
}
