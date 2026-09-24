using Fenicia.Auth.Domains.State.Interfaces;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.State;

/// <summary>
/// Repository implementation for managing state data.
/// </summary>
/// <param name="context">The database context.</param>
public class StateRepository(DbContext context) : Repository<StateModel>(context), IStateRepository
{
    /// <summary>
    /// Retrieves all states ordered by UF.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of all states ordered by UF.</returns>
    public Task<List<StateModel>> GetAllOrderedAsync(CancellationToken cancellationToken = default)
    {
        return DbSet
            .OrderBy(s => s.Uf)
            .ToListAsync(cancellationToken);
    }
}