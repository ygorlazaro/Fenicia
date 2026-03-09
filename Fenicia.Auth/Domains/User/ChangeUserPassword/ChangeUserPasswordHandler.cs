using Fenicia.Auth.Domains.Security.HashPassword;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.User.ChangeUserPassword;

public class ChangeUserPasswordHandler(
    DefaultContext context,
    HashPasswordHandler hashPasswordHandler)
{
    public virtual async Task<ChangeUserPasswordResponse> Handle(ChangeUserPasswordQuery request, CancellationToken ct)
    {
        var user = await context.AuthUsers
            .FirstOrDefaultAsync(u => u.Id == request.UserId, ct) ?? throw new InvalidRequestException(ExceptionMessages.UserNotFound);

        var hashedPassword = hashPasswordHandler.Handle(request.NewPassword);

        user.Password = hashedPassword;
        user.Updated = DateTime.UtcNow;

        await context.SaveChangesAsync(ct);

        return new ChangeUserPasswordResponse(true, "Password changed successfully");
    }
}
