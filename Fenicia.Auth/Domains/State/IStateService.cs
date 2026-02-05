using Fenicia.Common.Data.Responses.Auth;

namespace Fenicia.Auth.Domains.State;

public interface IStateService
{
    Task<List<StateResponse>> LoadStatesAtDatabaseAsync(CancellationToken cancellationToken);
}
