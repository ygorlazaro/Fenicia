namespace Fenicia.Auth.Domains.State;

using Common.Database.Contexts;
using Common.Database.Models.Auth;

public class StateRepository : IStateRepository
{
    private readonly AuthContext authContext;

    public StateRepository(AuthContext authContext)
    {
        this.authContext = authContext;
    }

    public async Task<List<StateModel>> LoadStatesAtDatabaseAsync(List<StateModel> states, CancellationToken cancellationToken)
    {
        this.authContext.States.AddRange(states);

        await this.authContext.SaveChangesAsync(cancellationToken);

        return states;
    }
}
