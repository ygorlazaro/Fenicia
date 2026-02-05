using Fenicia.Common.Database.Abstracts;
using Fenicia.Common.Database.Models.Auth;

namespace Fenicia.Auth.Domains.State;

public interface IStateRepository: IBaseRepository<StateModel>
{
    Task<List<StateModel>> LoadStatesAtDatabaseAsync(List<StateModel> states, CancellationToken cancellationToken);
}
