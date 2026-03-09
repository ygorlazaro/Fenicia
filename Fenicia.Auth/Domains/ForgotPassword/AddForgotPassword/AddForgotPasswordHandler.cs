using Fenicia.Auth.Domains.User;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;

namespace Fenicia.Auth.Domains.ForgotPassword.AddForgotPassword;

public class AddForgotPasswordHandler(DefaultContext db)
{
    public virtual async Task Handle(AddForgotPasswordCommand command, CancellationToken ct)
    {
        var userId = await db.UserIdByEmailAsync(command.Email, ct)
                     ?? throw new ItemNotExistsException(ExceptionMessages.UserWithEmailNotFound);
        var code = Guid.NewGuid().ToString().Replace("-", string.Empty)[..6];

        await db.AuthForgottenPasswords.AddAsync(new ForgotPasswordModel
        {
            Code = code,
            IsActive = true,
            UserId = userId
        }, ct);

        await db.SaveChangesAsync(ct);
    }
}
