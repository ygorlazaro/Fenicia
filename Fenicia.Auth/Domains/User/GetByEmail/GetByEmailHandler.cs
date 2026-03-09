using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.User.GetByEmail;

public class GetByEmailHandler(DefaultContext context)
{
    public virtual async Task<GetByEmailResponse?> Handle(string email, CancellationToken ct)
    {

        return await context.AuthUsers.Where(user => user.Email == email)
            .Select(user => new GetByEmailResponse(user.Id, user.Email, user.Name, user.Password)).FirstOrDefaultAsync(ct);
    }
}
