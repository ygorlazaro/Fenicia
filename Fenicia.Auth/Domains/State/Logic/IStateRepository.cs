namespace Fenicia.Auth.Domains.State.Logic;

using Fenicia.Auth.Domains.State.Data;

public interface IStateRepository
{
    Task<List<StateModel>> LoadStatesAtDatabaseAsync(List<StateModel> states, CancellationToken cancellationToken);
}
