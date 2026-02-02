using Fenicia.Common.Database.Models.Auth;

namespace Fenicia.Auth.Domains.State;

public interface IStateRepository
{
    Task<List<StateModel>> LoadStatesAtDatabaseAsync(List<StateModel> states, CancellationToken cancellationToken);
}
