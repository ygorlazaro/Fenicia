using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.UserRole.GetRolesByUser;

public class GetRolesByUserHandler(AuthContext context)
{
    public async Task<string[]> Handler(GetRolesByUserQuery query, CancellationToken ct)
    {
        return await context.UserRoles.Where(ur => ur.UserId == query.UserId).Select(ur => ur.Role.Name).ToArrayAsync(ct);
    }
}