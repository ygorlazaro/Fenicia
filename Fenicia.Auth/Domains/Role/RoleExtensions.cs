using Fenicia.Common.Data.Models.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Role;

public static class RoleExtensions
{
    public async static Task<RoleModel?> GetRoleAsync(this DbSet<RoleModel> db, string roleName, CancellationToken ct)
    {
        return await db.Where(r => r.Name == roleName).FirstOrDefaultAsync(ct);
    }
}