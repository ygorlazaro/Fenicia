namespace Fenicia.Auth.Domains.State.Logic;

using Data;

public interface IStateService
{
    Task<List<StateResponse>> LoadStatesAtDatabaseAsync(CancellationToken cancellationToken);
}
