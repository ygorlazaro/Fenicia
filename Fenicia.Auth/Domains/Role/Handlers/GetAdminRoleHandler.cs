using Fenicia.Auth.Domains.Role.Responses;
using Fenicia.Common.Data.Contexts;

namespace Fenicia.Auth.Domains.Role.Handlers;

public class GetAdminRoleHandler(DefaultContext db)
{
    public virtual async Task<GetAdminRoleResponse?> Handle(CancellationToken ct)
    {
        var role = await db.AuthRoles.GetRoleAsync("Admin", ct);

        return role switch
        {
            null => null,
            _ => new GetAdminRoleResponse(role.Id, role.Name)
        };
    }
}
