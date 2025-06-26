using Fenicia.Auth.Contexts;
using Fenicia.Auth.Domains.ForgotPassword.Data;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.ForgotPassword.Logic;

public class ForgotPasswordRepository(AuthContext authContext) : IForgotPasswordRepository
{
    public async Task<ForgotPasswordModel?> GetFromUserIdAndCodeAsync(Guid userId, string code,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var query = from forgotPassword in authContext.ForgottenPasswords
                    where forgotPassword.UserId == userId && forgotPassword.Code == code
                    && forgotPassword.IsActive
                    && forgotPassword.ExpirationDate >= now
                    select forgotPassword;

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task InvalidateCodeAsync(Guid id, CancellationToken cancellationToken)
    {
        var forgotPassword =
            await authContext.ForgottenPasswords.FirstOrDefaultAsync(fp => fp.Id == id, cancellationToken);

        if (forgotPassword is null)
        {
            return;
        }

        forgotPassword.IsActive = false;
        await authContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<ForgotPasswordModel> SaveForgotPasswordAsync(
        ForgotPasswordModel forgotPassword, CancellationToken cancellationToken)
    {
        authContext.ForgottenPasswords.Add(forgotPassword);
        await authContext.SaveChangesAsync(cancellationToken);

        return forgotPassword;
    }
}
