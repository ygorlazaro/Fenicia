using Fenicia.Auth.Domains.Security;
using Fenicia.Auth.Domains.User.Commands;
using Fenicia.Auth.Domains.User.Responses;
using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.User.Handlers;

public class UpdatePasswordHandler(DefaultContext db)
{
    public async Task<UpdatePasswordResponse> Handle(UpdatePasswordCommand command, CancellationToken ct)
    {
        var user = await db.AuthUsers.ExisingAsync(command.UserId , ct);
        var hashedPassword = command.Password.Hash();

        user.Password = hashedPassword;

        db.Entry(user).State = EntityState.Modified;

        await db.SaveChangesAsync(ct);

        return new UpdatePasswordResponse(user.Id, user.Name, user.Email);
    }
}
