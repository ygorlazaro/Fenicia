using Fenicia.Common.Data.Models.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.UserRole;

public static class UserRoleExtensions
{
    extension(DbSet<UserRoleModel> dbSet)
    {
        public async Task<bool> AnyIdAndCompanyAsync(Guid userId, Guid companyId, CancellationToken ct)
        {
            return await dbSet.AnyAsync(u => u.CompanyId == companyId && u.UserId == userId, ct);
        }

        public async Task<bool> HasRoleAsync(Guid userId, Guid companyId, string role, CancellationToken ct)
        {
            var query = dbSet.Where(ur => ur.UserId == userId && ur.CompanyId == companyId && ur.Role.Name == role).Select(ur => 1);

            return await query.AnyAsync(ct);
        }
    }
}