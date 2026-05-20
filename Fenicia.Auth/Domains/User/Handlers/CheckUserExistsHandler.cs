using Fenicia.Auth.Domains.User.Queries;
using Fenicia.Common.Data.Contexts;

using MediatR;

namespace Fenicia.Auth.Domains.User.Handlers;

public class CheckUserExistsHandler(DefaultContext db) : IRequestHandler<CheckUserExistsQuery, bool>
{
    public virtual async Task<bool> Handle(CheckUserExistsQuery query, CancellationToken ct)
    {
        return await db.AuthUsers.AnyEmailAsync(query.Email, ct);
    }

    public virtual Task<bool> Handle(string email, CancellationToken ct)
    {
        return Handle(new CheckUserExistsQuery(email), ct);
    }
}
