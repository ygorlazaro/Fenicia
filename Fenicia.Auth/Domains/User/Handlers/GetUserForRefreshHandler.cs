using Fenicia.Auth.Domains.User.Queries;
using Fenicia.Auth.Domains.User.Responses;
using Fenicia.Common.Data.Contexts;

using MediatR;

namespace Fenicia.Auth.Domains.User.Handlers;

public class GetUserForRefreshHandler(DefaultContext db) : IRequestHandler<GetUserForRefreshQuery, GetUserForRefreshResponse>
{
    public async Task<GetUserForRefreshResponse> Handle(GetUserForRefreshQuery query, CancellationToken ct)
    {
        var user = await db.AuthUsers.FirstByIdAsync(query.UserId, ct);

        return new GetUserForRefreshResponse(user.Id, user.Email, user.Name);
    }

    public Task<GetUserForRefreshResponse> Handle(Guid userId, CancellationToken ct)
    {
        return Handle(new GetUserForRefreshQuery(userId), ct);
    }
}
