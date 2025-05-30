using Fenicia.Auth.Contexts;
using Fenicia.Auth.Contexts.Models;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Repositories;

public class RoleRepository(AuthContext authContext) : IRoleRepository
{
    public async Task<RoleModel?> GetAdminRoleAsync()
    {
        var query = from role in authContext.Roles
            where role.Name == "Admin"
            select role;

        return await query.FirstOrDefaultAsync();
    }
}