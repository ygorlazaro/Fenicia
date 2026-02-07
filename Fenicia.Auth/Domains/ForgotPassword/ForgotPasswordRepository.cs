using Fenicia.Common.Data.Abstracts;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.ForgotPassword;

public class ForgotPasswordRepository(AuthContext context) : BaseRepository<ForgotPasswordModel>(context), IForgotPasswordRepository
{
    public async Task<ForgotPasswordModel?> GetFromUserIdAndCodeAsync(Guid userId, string code, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var query = context.ForgottenPasswords
            .Where(fp => fp.UserId == userId && fp.Code == code && fp.IsActive && fp.ExpirationDate >= now);

        return await query.FirstOrDefaultAsync(ct);
    }

    public async Task InvalidateCodeAsync(Guid id, CancellationToken ct)
    {
        var forgotPassword = await context.ForgottenPasswords.FirstOrDefaultAsync(fp => fp.Id == id, ct);

        if (forgotPassword is null)
        {
            return;
        }

        forgotPassword.IsActive = false;

        context.Entry(forgotPassword).State = EntityState.Modified;

        await context.SaveChangesAsync(ct);
    }

    public async Task<ForgotPasswordModel> SaveForgotPasswordAsync(ForgotPasswordModel forgotPassword, CancellationToken ct)
    {
        context.ForgottenPasswords.Add(forgotPassword);
        await context.SaveChangesAsync(ct);

        return forgotPassword;
    }
}
