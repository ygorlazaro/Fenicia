using Fenicia.Common.Data.Contexts;

namespace Fenicia.Auth.Domains.User.Handlers;

public class CheckUserExistsHandler(DefaultContext db)
{
    public virtual async Task<bool> Handle(string email, CancellationToken ct)
    {
        return await db.AuthUsers.AnyEmailAsync(email, ct);
    }
}