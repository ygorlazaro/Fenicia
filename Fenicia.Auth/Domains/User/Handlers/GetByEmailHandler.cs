using Fenicia.Auth.Domains.User.Queries;
using Fenicia.Auth.Domains.User.Responses;
using Fenicia.Common.Data.Contexts;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.User.Handlers;

public class GetByEmailHandler(DefaultContext db) : IRequestHandler<GetByEmailQuery, GetByEmailResponse?>
{
    public virtual async Task<GetByEmailResponse?> Handle(GetByEmailQuery query, CancellationToken ct)
    {
        return await db.AuthUsers.Where(user => user.Email == query.Email)
            .Select(user => new GetByEmailResponse(user.Id,
                user.Email,
                user.Name,
                user.Password))
            .FirstOrDefaultAsync(ct);
    }

    public virtual Task<GetByEmailResponse?> Handle(string email, CancellationToken ct)
    {
        return Handle(new GetByEmailQuery(email), ct);
    }
}
