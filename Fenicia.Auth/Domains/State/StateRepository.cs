using Fenicia.Common.Database.Abstracts;
using Fenicia.Common.Database.Contexts;
using Fenicia.Common.Database.Models.Auth;

namespace Fenicia.Auth.Domains.State;

public class StateRepository(AuthContext context) : BaseRepository<StateModel>(context), IStateRepository
{
    public async Task<List<StateModel>> LoadStatesAtDatabaseAsync(List<StateModel> states, CancellationToken cancellationToken)
    {
        context.States.AddRange(states);

        await context.SaveChangesAsync(cancellationToken);

        return states;
    }
}
