using Fenicia.Auth.Domains.ForgotPassword.Commands;
using Fenicia.Auth.Domains.Security;
using Fenicia.Auth.Domains.User;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.ForgotPassword.Handlers;

/// <summary>
///     Handler responsible for completing the password reset process.
///     Validates the reset code and updates the user's password.
/// </summary>
public class ResetPasswordHandler(DefaultContext db) : IRequestHandler<ResetPasswordCommand>
{
    /// <summary>
    ///     Handles the password reset request.
    ///     Validates the code, updates the user's password, and invalidates the used code.
    /// </summary>
    /// <param name="command">The command containing email, new password, and reset code.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ItemNotExistsException">Thrown when no user exists with the given email.</exception>
    /// <exception cref="InvalidDataException">Thrown when the code is invalid, expired, or already used.</exception>
    public virtual async Task Handle(ResetPasswordCommand command, CancellationToken ct)
    {
        var user = await db.AuthUsers.FirstByEmailOrDefaultAsync(command.Email, ct) ?? throw new ItemNotExistsException(ExceptionMessages.UserWithEmailNotFound);
        var currentCode = await GetFromUserIdAndCodeAsync(user.Id, command.Code, ct) ?? throw new InvalidDataException(ExceptionMessages.InvalidForgotPasswordCode);

        user.Password = command.Password.Hash();

        await InvalidateCodeAsync(currentCode.Id, ct);

        await db.SaveChangesAsync(ct);
    }

    /// <summary>
    ///     Retrieves an active, non-expired forgot password code for the given user.
    /// </summary>
    /// <param name="userId">The user ID to search for.</param>
    /// <param name="code">The reset code to validate.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The forgot password record if valid, otherwise null.</returns>
    private async Task<ForgotPasswordModel?> GetFromUserIdAndCodeAsync(Guid userId, string code, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var query = db.AuthForgottenPasswords.Where(fp => fp.UserId == userId && fp.Code == code && fp.IsActive && fp.ExpirationDate >= now);

        return await query.FirstOrDefaultAsync(ct);
    }

    /// <summary>
    ///     Invalidates a forgot password code by setting IsActive to false.
    /// </summary>
    /// <param name="id">The ID of the forgot password record to invalidate.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task InvalidateCodeAsync(Guid id, CancellationToken ct)
    {
        var forgotPassword = await db.AuthForgottenPasswords.FirstOrDefaultAsync(fp => fp.Id == id, ct);

        if (forgotPassword is null)
        {
            return;
        }

        forgotPassword.IsActive = false;

        db.Entry(forgotPassword).State = EntityState.Modified;
    }
}
