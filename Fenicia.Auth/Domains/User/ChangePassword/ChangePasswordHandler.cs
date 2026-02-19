using Fenicia.Auth.Domains.Security.HashPassword;
using Fenicia.Common;
using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.User.ChangePassword;

public class ChangePasswordHandler(AuthContext context, HashPasswordHandler hashPasswordHandler)
{
    public async Task<ChangePasswordResponse> Handle(ChangePasswordQuery query, CancellationToken ct)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id ==  query.Id, ct)
                   ?? throw new ArgumentException(TextConstants.ItemNotFoundMessage);
        var hashedPassword = hashPasswordHandler.Handle(query.Password);

        user.Password = hashedPassword;
        
        context.Entry(user).State = EntityState.Modified;

        await context.SaveChangesAsync(ct);

        return new ChangePasswordResponse(user.Id, user.Name, user.Email);
    }
}