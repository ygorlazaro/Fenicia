using Fenicia.Common;
using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.User.GetUserForRefresh;

public class GetUserForRefreshHandler(AuthContext context)
{
    public async Task<GetUserForRefreshResponse> Handle(Guid userId, CancellationToken ct)
    {
        var query = from u in context.Users
                    where u.Id == userId
                    select new GetUserForRefreshResponse(u.Id, u.Email, u.Name);

        var user = await query.FirstOrDefaultAsync(ct);

        return user ?? throw new UnauthorizedAccessException(TextConstants.PermissionDeniedMessage);

    }
}