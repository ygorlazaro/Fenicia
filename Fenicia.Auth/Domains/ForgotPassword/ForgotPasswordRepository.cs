using Fenicia.Common.Database.Contexts;
using Fenicia.Common.Database.Models.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.ForgotPassword;

public class ForgotPasswordRepository(AuthContext context) : IForgotPasswordRepository
{
    public async Task<ForgotPasswordModel?> GetFromUserIdAndCodeAsync(Guid userId, string code, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var query = context.ForgottenPasswords
            .Where(fp => fp.UserId == userId && fp.Code == code && fp.IsActive && fp.ExpirationDate >= now);

        var result = await query.FirstOrDefaultAsync(cancellationToken);

        return result;
    }

    public async Task InvalidateCodeAsync(Guid id, CancellationToken cancellationToken)
    {
        var forgotPassword = await context.ForgottenPasswords.FirstOrDefaultAsync(fp => fp.Id == id, cancellationToken);

        if (forgotPassword is null)
        {
            return;
        }

        forgotPassword.IsActive = false;
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<ForgotPasswordModel> SaveForgotPasswordAsync(ForgotPasswordModel forgotPassword, CancellationToken cancellationToken)
    {
        context.ForgottenPasswords.Add(forgotPassword);
        await context.SaveChangesAsync(cancellationToken);

        return forgotPassword;
    }
}
