using Fenicia.Common.Data.Abstracts;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;

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
