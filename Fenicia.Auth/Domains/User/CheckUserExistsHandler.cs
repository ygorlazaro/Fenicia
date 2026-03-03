using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.User;

public class CheckUserExistsHandler(DefaultContext context)
{
    public virtual async Task<bool> Handle(string email, CancellationToken ct)
    {
        return await context.AuthUsers.FirstOrDefaultAsync(u => u.Email == email, ct) != null;
    }
}
