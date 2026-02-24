using Fenicia.Auth.Domains.User;
using Fenicia.Auth.Domains.User.ChangePassword;
using Fenicia.Common;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.ForgotPassword.ResetPassword;

public class ResetPasswordHandler(AuthContext db, ChangePasswordHandler changePasswordHandler)
{
    public virtual async Task Handle(ResetPasswordCommand command, CancellationToken ct)
    {
        var userId = await db.UserIdByEmailAsync(command.Email, ct)
                     ?? throw new ItemNotExistsException(TextConstants.ItemNotFoundMessage);
        var currentCode = await GetFromUserIdAndCodeAsync(userId, command.Code, ct)
                          ?? throw new InvalidDataException(TextConstants.InvalidForgetCode);

        await changePasswordHandler.Handle(new ChangePasswordQuery(currentCode.UserId, command.Password), ct);
        await InvalidateCodeAsync(currentCode.Id, ct);
    }

    private async Task<ForgotPasswordModel?> GetFromUserIdAndCodeAsync(Guid userId, string code, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var query = db.ForgottenPasswords
            .Where(fp => fp.UserId == userId && fp.Code == code && fp.IsActive && fp.ExpirationDate >= now);

        return await query.FirstOrDefaultAsync(ct);
    }

    private async Task InvalidateCodeAsync(Guid id, CancellationToken ct)
    {
        var forgotPassword = await db.ForgottenPasswords.FirstOrDefaultAsync(fp => fp.Id == id, ct);

        if (forgotPassword is null)
        {
            return;
        }

        forgotPassword.IsActive = false;

        db.Entry(forgotPassword).State = EntityState.Modified;

        await db.SaveChangesAsync(ct);
    }
}