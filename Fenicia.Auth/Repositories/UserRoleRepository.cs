using Fenicia.Auth.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Repositories;

public class UserRoleRepository(AuthContext context) : IUserRoleRepository
{
    public async Task<string[]> GetRolesByUserAsync(Guid userId)
    {
        return await context.UserRoles.Where(ur => ur.UserId == userId).Select(ur => ur.Role.Name).ToArrayAsync();
    }
}