using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.User.DeleteUser;

public class DeleteUserHandler(DefaultContext context)
{
    public virtual async Task<DeleteUserResponse> Handle(DeleteUserQuery request, CancellationToken ct)
    {
        var user = await context.AuthUsers
            .Include(u => u.UsersRoles)
            .FirstOrDefaultAsync(u => u.Id == request.UserId, ct) ?? throw new InvalidRequestException(ExceptionMessages.UserNotFound);

        user.Deleted = DateTime.UtcNow;

        await context.SaveChangesAsync(ct);

        return new DeleteUserResponse(true, "User deleted successfully");
    }
}
