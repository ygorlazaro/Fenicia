using Fenicia.Auth.Contexts;
using Fenicia.Auth.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Repositories;

public class UserRoleRepository(AuthContext context) : IUserRoleRepository
{
    public async Task<string[]> GetRolesByUserAsync(Guid userId)
    {
        return await context.UserRoles.Where(ur => ur.UserId == userId).Select(ur => ur.Role.Name).ToArrayAsync();
    }

    public async Task<bool> ExistsInCompanyAsync(Guid userId, Guid companyId)
    {
        return await context.UserRoles.AnyAsync(ur => ur.UserId == userId && ur.CompanyId == companyId);
    }
}