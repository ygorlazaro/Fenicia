using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.User;

public static class UserQueries
{
    extension(DefaultContext db)
    {
        public async Task<Guid?> UserIdByEmailAsync(string email, CancellationToken ct)
        {
            var result = await db.AuthUsers.Where(u => u.Email == email).Select(u => u.Id).FirstOrDefaultAsync(ct);

            return Guid.Empty == result ? null : result;
        }

        public async Task<bool> UserExistsAsync(
            Guid userId,
            Guid companyId,
            CancellationToken ct)
        {
            return await db.UserRoles.AnyAsync(ur => ur.UserId == userId && ur.CompanyId == companyId, ct);
        }
    }
}
