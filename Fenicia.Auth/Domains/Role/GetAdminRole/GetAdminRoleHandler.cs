using Fenicia.Common.Data.Contexts;

namespace Fenicia.Auth.Domains.Role.GetAdminRole;

public class GetAdminRoleHandler(DefaultContext context)
{
    public virtual async Task<GetAdminRoleResponse?> Handle(CancellationToken ct)
    {
        var role = await context.AuthRoles.GetRoleAsync("Admin", ct);

        return role == null ? null : new GetAdminRoleResponse(role.Id, role.Name);

    }
}
