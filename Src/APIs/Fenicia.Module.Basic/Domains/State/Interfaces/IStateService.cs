using Fenicia.Module.Basic.Domains.State.DTOs;

namespace Fenicia.Module.Basic.Domains.State.Interfaces;

public interface IStateService
{
    Task<List<GetAllStateResponse>> GetAllAsync(GetAllStateQuery query, CancellationToken cancellationToken = default);
}
