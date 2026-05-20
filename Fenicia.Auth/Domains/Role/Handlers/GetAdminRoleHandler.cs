using Fenicia.Auth.Domains.Role.Queries;
using Fenicia.Auth.Domains.Role.Responses;
using Fenicia.Common.Data.Contexts;

using MediatR;

namespace Fenicia.Auth.Domains.Role.Handlers;

public class GetAdminRoleHandler(DefaultContext db) : IRequestHandler<GetAdminRoleQuery, GetAdminRoleResponse?>
{
    public virtual async Task<GetAdminRoleResponse?> Handle(GetAdminRoleQuery request, CancellationToken ct)
    {
        var role = await db.AuthRoles.GetRoleAsync("Admin", ct);

        return role switch
        {
            null => null,
            _ => new GetAdminRoleResponse(role.Id, role.Name)
        };
    }

    public virtual async Task<GetAdminRoleResponse?> Handle(CancellationToken ct)
    {
        return await Handle(new GetAdminRoleQuery(), ct);
    }
}
