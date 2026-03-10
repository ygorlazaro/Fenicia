using Fenicia.Auth.Domains.User.Queries;
using Fenicia.Auth.Domains.User.Responses;
using Fenicia.Common;
using Fenicia.Common.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.User.Handlers;

public class GetUserHandler(
    DefaultContext db)
{
    public virtual async Task<Pagination<List<UserListItemResponse>>> Handle(GetUsersQuery query, CancellationToken ct)
    {
        var request = db.AuthUsers.OrderBy(u => u.Name);
        var totalCount = await request.CountAsync(ct);

        var users = await request
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .Select(u => new UserListItemResponse(
                u.Id,
                u.Name,
                u.Email
            ))
            .ToListAsync(ct);

        return new Pagination<List<UserListItemResponse>>(users, totalCount, query.Page, query.PerPage);
    }
}
