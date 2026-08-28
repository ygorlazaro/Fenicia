using Fenicia.Auth.Domains.ForgotPassword.DTOs;
using Fenicia.Auth.Domains.Security;
using Fenicia.Auth.Domains.User;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.ForgotPassword;

public class ForgotPasswordService(DefaultContext db, UserService userService)
{
    public async Task AddAsync(AddForgotPasswordCommand command, CancellationToken ct)
    {
        var user = await userService.FirstByEmailOrDefaultAsync(command.Email, ct) ?? throw new ItemNotExistsException(ExceptionMessages.UserWithEmailNotFound);
        var code = Guid.NewGuid().ToString().Replace("-", string.Empty)[..6];

        var forgotPasswordModel = new ForgotPasswordModel
        {
            Code = code,
            IsActive = true,
            UserId = user.Id,
            IpAddress = command.IpAddress,
            UserAgent = command.UserAgent
        };

        await db.AuthForgottenPasswords.AddAsync(forgotPasswordModel, ct);
        await db.SaveChangesAsync(ct);
    }

    public async Task ResetAsync(ResetPasswordCommand command, CancellationToken ct)
    {
        var user = await userService.FirstByEmailOrDefaultAsync(command.Email, ct) ?? throw new ItemNotExistsException(ExceptionMessages.UserWithEmailNotFound);
        var currentCode = await GetFromUserIdAndCodeAsync(user.Id, command.Code, ct) ?? throw new InvalidDataException(ExceptionMessages.InvalidForgotPasswordCode);

        user.Password = SecurityService.Hash(command.Password);

        await InvalidateCodeAsync(currentCode.Id, ct);

        await db.SaveChangesAsync(ct);
    }

    private async Task<ForgotPasswordModel?> GetFromUserIdAndCodeAsync(Guid userId, string code, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var query = db.AuthForgottenPasswords.Where(fp => fp.UserId == userId && fp.Code == code && fp.IsActive && fp.ExpirationDate >= now);

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
    }
}
