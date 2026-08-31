using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;

namespace Fenicia.Module.Basic.Domains.State;

public interface IStateRepository : IRepository<StateModel>
{
    Task<List<StateModel>> GetAllOrderedAsync(CancellationToken ct);
}
