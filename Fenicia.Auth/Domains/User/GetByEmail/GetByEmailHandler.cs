using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.User.GetByEmail;

public class GetByEmailHandler(AuthContext context)
{
    private const int ExpirationMinutes = 15;
    
    public async Task<GetByEmailResponse?> Handle(string email, CancellationToken ct)
    {
        var query = from user in context.Users
                    where user.Email == email
                    select new GetByEmailResponse(user.Id, user.Email, user.Name, user.Password);

        return await query.FirstOrDefaultAsync(ct);
    }
}