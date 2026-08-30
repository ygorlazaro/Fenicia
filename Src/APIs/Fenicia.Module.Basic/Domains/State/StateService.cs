using Fenicia.Common;
using Fenicia.Common.Data.Models.Basic;
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

    public virtual async Task<List<GetAllStateResponse>> GetAllAsync(GetAllStateQuery query, CancellationToken ct)
    {
        var states = await _stateRepository.GetAllOrderedAsync(ct);

        return [.. states.Select(s => s.MapToGetAllStateResponse())];
    }
}
