using Fenicia.Auth.Contexts;
using Fenicia.Auth.Domains.User.Data;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.User.Logic;

public class UserRepository(AuthContext authContext) : IUserRepository
{
    public async Task<UserModel?> GetByEmailAndCnpjAsync(string email, string cnpj, CancellationToken cancellationToken)
    {
        var query =
            from user in authContext.Users
            join userRole in authContext.UserRoles on user.Id equals userRole.UserId
            join company in authContext.Companies on userRole.CompanyId equals company.Id
            where user.Email == email && company.Cnpj == cnpj
            select user;

        return await query.FirstOrDefaultAsync(cancellationToken: cancellationToken);
    }

    public UserModel Add(UserModel userRequest)
    {
        authContext.Users.Add(userRequest);

        return userRequest;
    }

    public async Task<int> SaveAsync(CancellationToken cancellationToken)
    {
        return await authContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> CheckUserExistsAsync(string email, CancellationToken cancellationToken)
    {
        return await authContext.Users.AnyAsync(u => u.Email == email, cancellationToken: cancellationToken);
    }

    public async Task<UserModel?> GetUserForRefreshTokenAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await authContext.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken: cancellationToken);
    }

    public async Task<Guid?> GetUserIdFromEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await authContext.Users.Select(u => u.Id).FirstOrDefaultAsync(cancellationToken: cancellationToken);
    }

    public async Task<UserModel?> GetByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await authContext.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken: cancellationToken);
    }
}
