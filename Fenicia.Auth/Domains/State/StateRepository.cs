namespace Fenicia.Auth.Domains.State;

using Common.Database.Contexts;
using Common.Database.Models.Auth;

public class StateRepository : IStateRepository
{
    private readonly AuthContext _authContext;

    public StateRepository(AuthContext authContext)
    {
        _authContext = authContext;
    }

    public async Task<List<StateModel>> LoadStatesAtDatabaseAsync(List<StateModel> states, CancellationToken cancellationToken)
    {
        _authContext.States.AddRange(states);

        await _authContext.SaveChangesAsync(cancellationToken);

        return states;
    }
}
