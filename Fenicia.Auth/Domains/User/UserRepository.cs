using Fenicia.Auth.Domains.User.Interfaces;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.User;

public class UserRepository(DbContext context) : Repository<UserModel>(context), IUserRepository
{
    public override async Task<IEnumerable<UserModel>> GetAllAsync(int page = 1, int perPage = 10, CancellationToken cancellationToken = default)
    {
        var query = from u in DbSet
            orderby u.Name
            select u;

        return await query
            .Skip((page - 1) *  perPage)
            .Take(perPage)
            .ToListAsync(cancellationToken);
    }

    public Task<UserModel?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return DbSet
            .Include(u => u.UsersRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return DbSet.AnyAsync(u => u.Email == email, cancellationToken);
    }
}