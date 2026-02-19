using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Role.GetAdminRole;

public class GetAdminRoleHandler(AuthContext context)
{
    public async Task<GetAdminRoleResponse?> Handle(CancellationToken ct)
    {
        return await context.Roles.Where(role => role.Name == "Admin")
            .Select(r => new GetAdminRoleResponse(r.Id, r.Name))
            .FirstOrDefaultAsync(ct);
    }   
}