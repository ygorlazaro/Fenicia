using Fenicia.Common.Data.Abstracts;
using Fenicia.Common.Data.Contexts;

using Fenicia.Common.Data.Models.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.User;

public class UserRepository(AuthContext context) : BaseRepository<UserModel>(context), IUserRepository
{
    public async Task<UserModel?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public override void Add(UserModel userRequest)
    {
        context.Users.Add(userRequest);

        foreach (var role in userRequest.UsersRoles)
        {
            context.Entry(role.Role).State = EntityState.Unchanged;
        }
    }

    public async Task<bool> CheckUserExistsAsync(string email, CancellationToken cancellationToken)
    {
        return await context.Users.AnyAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<Guid?> GetUserIdFromEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await context.Users.Where(u => u.Email == email).Select(u => u.Id).FirstOrDefaultAsync(cancellationToken);
    }
}
