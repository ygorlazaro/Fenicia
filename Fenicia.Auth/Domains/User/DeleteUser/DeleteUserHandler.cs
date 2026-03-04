using Fenicia.Auth.Domains.User.DeleteUser;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.User.DeleteUser;

public class DeleteUserHandler(DefaultContext context)
{
    public virtual async Task<DeleteUserResponse> Handle(DeleteUserQuery request, CancellationToken ct)
    {
        // Find user
        var user = await context.AuthUsers
            .Include(u => u.UsersRoles)
            .FirstOrDefaultAsync(u => u.Id == request.UserId, ct);

        if (user == null)
        {
            throw new ArgumentException("User not found");
        }

        // Soft delete - set Deleted timestamp
        user.Deleted = DateTime.UtcNow;

        // Optionally: Remove user roles (or keep them for audit purposes)
        // context.AuthUsersRoles.RemoveRange(user.UsersRoles);

        await context.SaveChangesAsync(ct);

        return new DeleteUserResponse(true, "User deleted successfully");
    }
}
