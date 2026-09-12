using Fenicia.Common.DTOs.Basic.State;

namespace Fenicia.Module.Basic.Domains.State.Interfaces;

public interface IStateService
{
    Task<List<GetAllStateResponse>> GetAllAsync(GetAllStateQuery query, CancellationToken cancellationToken = default);
}