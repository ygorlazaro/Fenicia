using Fenicia.Auth.Domains.User.Commands;
using Fenicia.Auth.Domains.User.Handlers;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.ForgotPassword.ResetPassword;

public class ResetPasswordHandler(DefaultContext db, UpdatePasswordHandler updatePasswordHandler)
{
    public virtual async Task Handle(ResetPasswordCommand command, CancellationToken ct)
    {
        var user = await UserIdByEmailAsync(command.Email, ct)
                     ?? throw new ItemNotExistsException(ExceptionMessages.UserWithEmailNotFound);
        var currentCode = await GetFromUserIdAndCodeAsync(user.Id, command.Code, ct)
                          ?? throw new InvalidDataException(ExceptionMessages.InvalidForgotPasswordCode);

        await updatePasswordHandler.Handle(new UpdatePasswordCommand(currentCode.UserId, command.Password), ct);
        await InvalidateCodeAsync(currentCode.Id, ct);
    }

    private async Task<UserModel?> UserIdByEmailAsync(string email, CancellationToken ct)
    {
        return await db.AuthUsers
            .Where(u => u.Email == email)
            .FirstOrDefaultAsync(ct);
    }

    private async Task<ForgotPasswordModel?> GetFromUserIdAndCodeAsync(Guid userId, string code, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var query = db.AuthForgottenPasswords
            .Where(fp => fp.UserId == userId && fp.Code == code && fp.IsActive && fp.ExpirationDate >= now);

        return await query.FirstOrDefaultAsync(ct);
    }

    private async Task InvalidateCodeAsync(Guid id, CancellationToken ct)
    {
        var forgotPassword = await db.AuthForgottenPasswords.FirstOrDefaultAsync(fp => fp.Id == id, ct);

        if (forgotPassword is null)
        {
            return;
        }

        forgotPassword.IsActive = false;

        db.Entry(forgotPassword).State = EntityState.Modified;

        await db.SaveChangesAsync(ct);
    }
}
