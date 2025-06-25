using Fenicia.Auth.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.ForgotPassword;

public class ForgotPasswordRepository(AuthContext authContext) : IForgotPasswordRepository
{
    public async Task<ForgotPasswordModel?> GetFromUserIdAndCodeAsync(Guid userId, string code)
    {
        var now = DateTime.UtcNow;
        var query = from forgotPassword in authContext.ForgottenPasswords
                    where forgotPassword.UserId == userId && forgotPassword.Code == code
                    && forgotPassword.IsActive
                    && forgotPassword.ExpirationDate >= now
                    select forgotPassword;

        return await query.FirstOrDefaultAsync();
    }

    public async Task InvalidateCodeAsync(Guid id)
    {
        var forgotPassword =
            await authContext.ForgottenPasswords.FirstOrDefaultAsync(fp => fp.Id == id);

        if (forgotPassword is null)
        {
            return;
        }

        forgotPassword.IsActive = false;
        await authContext.SaveChangesAsync();
    }

    public async Task<ForgotPasswordModel> SaveForgotPasswordAsync(
        ForgotPasswordModel forgotPassword)
    {
        authContext.ForgottenPasswords.Add(forgotPassword);
        await authContext.SaveChangesAsync();

        return forgotPassword;
    }
}
