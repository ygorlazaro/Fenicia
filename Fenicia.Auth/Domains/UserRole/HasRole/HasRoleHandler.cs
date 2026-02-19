using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.UserRole.HasRole;

public class HasRoleHandler(AuthContext context)
{
    public async Task<bool> Handle(HasRoleQuery query, CancellationToken ct)
    {
        return await context.UserRoles.AnyAsync(
            ur => ur.UserId == query.UserId && ur.CompanyId == query.CompanyId && ur.Role.Name == query.Role, ct);
    }
}