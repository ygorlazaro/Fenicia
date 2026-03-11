using Fenicia.Common.Data.Models.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.UserRole;

public static class UserRoleExtensions
{
    public async static Task<bool> AnyIdAndCompanyAsync(this DbSet<UserRoleModel> dbSet, Guid userId, Guid companyId, CancellationToken ct)
    {
        return await dbSet.AnyAsync(u => u.CompanyId == companyId && u.UserId == userId, ct);
    }
}