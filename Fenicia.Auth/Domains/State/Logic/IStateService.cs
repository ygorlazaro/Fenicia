namespace Fenicia.Auth.Domains.State.Logic;

using Common.Database.Responses;

public interface IStateService
{
    Task<List<StateResponse>> LoadStatesAtDatabaseAsync(CancellationToken cancellationToken);
}
