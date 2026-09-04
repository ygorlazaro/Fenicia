using Fenicia.Auth.Domains.User.Interfaces;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.User;

public class UserRepository(DefaultContext context) : Repository<UserModel>(context), IUserRepository
{
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