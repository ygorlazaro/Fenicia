using Fenicia.Auth.Domains.Security;
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
            return await dbUser.FirstOrDefaultAsync(u => u.Id == userId, ct) ?? throw new InvalidRequestException(ExceptionMessages.UserNotFound);
        }

        public async Task<bool> AnyEmailAsync(string email, CancellationToken ct)
        {
            return await dbUser.AnyAsync(u => u.Email == email, ct);
        }

        public async Task<UserModel?> FirstByEmailOrDefaultAsync(string email, CancellationToken ct)
        {
            return await dbUser.FirstOrDefaultAsync(u => u.Email == email, ct);
        }

        public async Task<UserModel?> UpdatePasswordAsync(Guid userId, string plainPassword, CancellationToken ct)
        {
            var user = await dbUser.FirstByIdAsync(userId, ct);

            user.Password = plainPassword.Hash();

            return user;
        }
    }
}