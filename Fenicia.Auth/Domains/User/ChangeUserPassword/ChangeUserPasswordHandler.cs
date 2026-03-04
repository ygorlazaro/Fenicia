using Fenicia.Auth.Domains.Security.HashPassword;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.User.ChangeUserPassword;

public class ChangeUserPasswordHandler(
    DefaultContext context,
    HashPasswordHandler hashPasswordHandler)
{
    public virtual async Task<ChangeUserPasswordResponse> Handle(ChangeUserPasswordQuery request, CancellationToken ct)
    {
        // Find user
        var user = await context.AuthUsers
            .FirstOrDefaultAsync(u => u.Id == request.UserId, ct);

        if (user == null)
        {
            throw new ArgumentException("User not found");
        }

        // Hash new password
        var hashedPassword = hashPasswordHandler.Handle(request.NewPassword);

        // Update password
        user.Password = hashedPassword;
        user.Updated = DateTime.UtcNow;

        await context.SaveChangesAsync(ct);

        return new ChangeUserPasswordResponse(true, "Password changed successfully");
    }
}
