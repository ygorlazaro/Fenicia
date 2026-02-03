using Fenicia.Common.Database.Contexts;

using Fenicia.Common.Database.Models.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.User;

public class UserRepository(AuthContext context) : IUserRepository
{
    public async Task<UserModel?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public UserModel Add(UserModel userRequest)
    {
        context.Users.Add(userRequest);

        return userRequest;
    }

    public async Task<int> SaveAsync(CancellationToken cancellationToken)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> CheckUserExistsAsync(string email, CancellationToken cancellationToken)
    {
        return await context.Users.AnyAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<UserModel?> GetUserForRefreshTokenAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }

    public async Task<Guid?> GetUserIdFromEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await context.Users.Where(u => u.Email == email).Select(u => u.Id).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<UserModel?> GetByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }
}
