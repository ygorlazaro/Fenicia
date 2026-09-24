using Fenicia.Auth.Domains.User.Interfaces;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.User;

/// <summary>
/// Repository implementation for managing users in the authentication domain.
/// </summary>
/// <param name="context">The database context.</param>
public class UserRepository(DbContext context) : Repository<UserModel>(context), IUserRepository
{
    /// <summary>
    /// Gets all users with pagination, ordered by name.
    /// </summary>
    /// <param name="page">The page number.</param>
    /// <param name="perPage">The number of items per page.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with the list of users.</returns>
    public override async Task<IEnumerable<UserModel>> GetAllAsync(int page = 1, int perPage = 10, CancellationToken cancellationToken = default)
    {
        var query = from u in DbSet
            orderby u.Name
            select u;

        return await query
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets a user by email address with roles included.
    /// </summary>
    /// <param name="email">The email address to search for.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with the user model if found.</returns>
    public Task<UserModel?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return DbSet
            .Include(u => u.UsersRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    /// <summary>
    /// Checks if a user exists by email address.
    /// </summary>
    /// <param name="email">The email address to check.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with true if user exists.</returns>
    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return DbSet.AnyAsync(u => u.Email == email, cancellationToken);
    }
}