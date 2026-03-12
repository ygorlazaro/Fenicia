using Fenicia.Auth.Domains.ForgotPassword.Commands;
using Fenicia.Auth.Domains.User;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;

namespace Fenicia.Auth.Domains.ForgotPassword.Handlers;

public class AddForgotPasswordHandler(DefaultContext db)
{
    public virtual async Task Handle(AddForgotPasswordCommand command, CancellationToken ct)
    {
        var user = await db.AuthUsers.FirstByEmailOrDefaultAsync(command.Email,
            ct) ?? throw new ItemNotExistsException(ExceptionMessages.UserWithEmailNotFound);
        var code = Guid.NewGuid().ToString().Replace("-",
            string.Empty)[..6];

        var forgotPasswordModel = new ForgotPasswordModel
        {
            Code = code,
            IsActive = true,
            UserId = user.Id
        };
        
        await db.AuthForgottenPasswords.AddAsync(forgotPasswordModel,
            ct);
        await db.SaveChangesAsync(ct);
    }
}
