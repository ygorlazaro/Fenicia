using Fenicia.Common.Database.Responses;

namespace Fenicia.Auth.Domains.State;

public interface IStateService
{
    Task<List<StateResponse>> LoadStatesAtDatabaseAsync(CancellationToken cancellationToken);
}
