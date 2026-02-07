using Fenicia.Common.Data.Models.Basic;

namespace Fenicia.Module.Basic.Domains.State;

public interface IStateService
{
    Task<List<StateModel>> GetAllAsync(CancellationToken ct);
}