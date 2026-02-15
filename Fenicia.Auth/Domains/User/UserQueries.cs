using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.User;

public static class UserQueries
{
    public async static Task<Guid?> UserIdByEmailAsync(this AuthContext db, string email, CancellationToken ct)
    {
        return await db.Users.Where(u => u.Email == email).Select(u => u.Id).FirstOrDefaultAsync(ct);
    }

    public async static Task<bool> UserExistsAsync(this AuthContext db, Guid userId, Guid companyId, CancellationToken ct)
    {
        return await db.UserRoles.AnyAsync(ur => ur.UserId == userId && ur.CompanyId == companyId, ct);
    }
}
