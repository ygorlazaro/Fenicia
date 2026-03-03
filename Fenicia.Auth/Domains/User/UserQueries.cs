using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.User;

public static class UserQueries
{
    public async static Task<Guid?> UserIdByEmailAsync(this DefaultContext db, string email, CancellationToken ct)
    {
        var result = await db.AuthUsers.Where(u => u.Email == email).Select(u => u.Id).FirstOrDefaultAsync(ct);

        return Guid.Empty == result ? null : result;
    }

    public async static Task<bool> UserExistsAsync(
        this DefaultContext db,
        Guid userId,
        Guid companyId,
        CancellationToken ct)
    {
        return await db.UserRoles.AnyAsync(ur => ur.UserId == userId && ur.CompanyId == companyId, ct);
    }
}
