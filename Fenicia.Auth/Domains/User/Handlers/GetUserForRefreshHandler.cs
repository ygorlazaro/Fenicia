using Fenicia.Auth.Domains.User.Responses;
using Fenicia.Common.Data.Contexts;

namespace Fenicia.Auth.Domains.User.Handlers;

public class GetUserForRefreshHandler(DefaultContext db)
{
    public async Task<GetUserForRefreshResponse> Handle(Guid userId, CancellationToken ct)
    {
        var user = await db.AuthUsers.FirstByIdAsync(userId,
            ct);

        return new GetUserForRefreshResponse(user.Id,
            user.Email,
            user.Name);
    }
}
