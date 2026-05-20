using Fenicia.Auth.Domains.Security;
using Fenicia.Auth.Domains.User.Commands;
using Fenicia.Auth.Domains.User.Responses;
using Fenicia.Common.Data.Contexts;

using MediatR;

namespace Fenicia.Auth.Domains.User.Handlers;

public class UpdateUserPasswordHandler(DefaultContext db) : IRequestHandler<UpdateUserPasswordCommand, UpdateUserPasswordResponse>
{
    public virtual async Task<UpdateUserPasswordResponse> Handle(UpdateUserPasswordCommand command, CancellationToken ct)
    {
        var user = await db.AuthUsers.FirstByIdAsync(command.UserId, ct);
        var hashedPassword = command.Password.Hash();

        user.Password = hashedPassword;

        await db.SaveChangesAsync(ct);

        return new UpdateUserPasswordResponse(true, "Password changed successfully");
    }
}
