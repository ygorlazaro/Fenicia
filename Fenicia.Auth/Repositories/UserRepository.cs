using Fenicia.Auth.Contexts;
using Fenicia.Auth.Contexts.Models;
using Fenicia.Auth.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Repositories;

public class UserRepository(AuthContext authContext) : IUserRepository
{
    public async Task<UserModel?> GetByEmailAndCnpjAsync(string email, string cnpj)
    {
        var query = from user in authContext.Users
                    join userRole in authContext.UserRoles on user.Id equals userRole.UserId
                    join company in authContext.Companies on userRole.CompanyId equals company.Id
                    where user.Email == email
                          && company.Cnpj == cnpj
                    select user;

        return await query.FirstOrDefaultAsync();
    }

    public UserModel Add(UserModel userRequest)
    {
        userRequest.Created = DateTime.Now;
        authContext.Users.Add(userRequest);

        return userRequest;
    }

    public async Task<int> SaveAsync()
    {
        return await authContext.SaveChangesAsync();
    }

    public async Task<bool> CheckUserExistsAsync(string email)
    {
        return await authContext.Users.AnyAsync(u => u.Email == email);
    }

    public async Task<UserModel?> GetUserForRefreshTokenAsync(Guid userId)
    {
        return await authContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
    }
}