using Fenicia.Auth.Domains.User;
using Fenicia.Common;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;

namespace Fenicia.Auth.Domains.ForgotPassword.AddForgotPassword;

public sealed class ForgotPasswordHandler(AuthContext db)
{
    public async Task Handle(ForgotPasswordCommand command, CancellationToken ct)
    {
        var userId = await db.UserIdByEmailAsync(command.Email, ct) ?? throw new ItemNotExistsException(TextConstants.ItemNotFoundMessage);
        var code = Guid.NewGuid().ToString().Replace("-", string.Empty)[..6];

        await db.ForgottenPasswords.AddAsync(new ForgotPasswordModel()
        {
            Code = code,
            IsActive = true,
            UserId = userId
        }, ct);

        await db.SaveChangesAsync(ct);
    }
}
