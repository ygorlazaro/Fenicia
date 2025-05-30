using Fenicia.Auth.Contexts;
using Fenicia.Auth.Contexts.Models;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Repositories;

public class UserRepository(AuthContext authContext) : IUserRepository
{
    public async Task<UserModel?> GetByEmailAsync(string email)
    {
        return await authContext.Users.FirstOrDefaultAsync(u => u.Email == email);
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
}