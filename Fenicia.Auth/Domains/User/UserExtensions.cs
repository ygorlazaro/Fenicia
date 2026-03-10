using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.User;

public static class UserExtensions
{
    extension(DbSet<UserModel> dbUser)
    {
        public async Task<UserModel> FirstByIdAsync(Guid userId, CancellationToken ct)
        {
            return await dbUser.FirstOrDefaultAsync(u => u.Id == userId, ct)
                   ?? throw new InvalidRequestException(ExceptionMessages.UserNotFound);
        }

        public async Task<bool> AnyEmailAsync(string email, CancellationToken ct)
        {
            return await dbUser.AnyAsync(u => u.Email == email, ct);
        }
    }
}