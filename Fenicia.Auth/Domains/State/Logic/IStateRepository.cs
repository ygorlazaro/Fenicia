namespace Fenicia.Auth.Domains.State.Logic;

using Common.Database.Models.Auth;

public interface IStateRepository
{
    Task<List<StateModel>> LoadStatesAtDatabaseAsync(List<StateModel> states, CancellationToken cancellationToken);
}
