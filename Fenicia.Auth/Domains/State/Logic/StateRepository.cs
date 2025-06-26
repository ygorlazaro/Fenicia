namespace Fenicia.Auth.Domains.State.Logic;

using Contexts;

using Data;

public class StateRepository(AuthContext authContext) : IStateRepository
{
    public async Task<List<StateModel>> LoadStatesAtDatabaseAsync(List<StateModel> states, CancellationToken cancellationToken)
    {
        authContext.States.AddRange(states);

        await authContext.SaveChangesAsync(cancellationToken);

        return states;
    }
}
