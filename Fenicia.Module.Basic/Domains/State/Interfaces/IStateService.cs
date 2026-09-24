using Fenicia.Common.DTOs.Auth.State;

namespace Fenicia.Module.Basic.Domains.State.Interfaces;

public interface IStateService
{
    Task<List<GetAllStateResponse>> GetAllAsync(StateRequest query, CancellationToken cancellationToken = default);
}
