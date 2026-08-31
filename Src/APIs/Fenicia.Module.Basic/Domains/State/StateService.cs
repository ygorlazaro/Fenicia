using Fenicia.Common;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Repositories;
using Fenicia.Module.Basic.Domains.State.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.State;

public class StateService
{
    private readonly IStateRepository _stateRepository;

    public StateService()
        : this(null!)
    {
    }

    public StateService(IStateRepository stateRepository)
    {
        _stateRepository = stateRepository;
    }

    public virtual async Task<List<GetAllStateResponse>> GetAllAsync(GetAllStateQuery query, CancellationToken cancellationToken = default)
    {
        var baseQuery = _stateRepository.Query();

        var filters = AdvancedQueryParser.Parse(query.Query);
        var filteredQuery = baseQuery.ApplyAdvancedQuery(filters, query.Sort);

        var states = await filteredQuery
            .OrderBy(s => s.Uf)
            .ToListAsync(cancellationToken);

        return [.. states.Select(s => s.MapToGetAllStateResponse())];
    }
}
