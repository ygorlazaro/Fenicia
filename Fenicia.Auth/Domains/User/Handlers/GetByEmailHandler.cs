using Fenicia.Auth.Domains.User.Responses;
using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.User.Handlers;

public class GetByEmailHandler(DefaultContext db)
{
    public virtual async Task<GetByEmailResponse?> Handle(string email, CancellationToken ct)
    {
        return await db.AuthUsers.Where(user => user.Email == email)
            .Select(user => new GetByEmailResponse(user.Id,
                user.Email,
                user.Name,
                user.Password))
            .FirstOrDefaultAsync(ct);
    }
}
