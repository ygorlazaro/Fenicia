using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.User.ListUsers;

public class ListUsersHandler(DefaultContext context)
{
    public virtual async Task<ListUsersResponse> Handle(ListUsersQuery request, CancellationToken ct)
    {
        var query = context.AuthUsers
            .Include(u => u.UsersRoles)
                .ThenInclude(ur => ur.RoleModel)
            .Include(u => u.UsersRoles)
                .ThenInclude(ur => ur.CompanyModel)
            .AsQueryable();

        // Apply search filter if provided
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(u => u.Name.Contains(request.SearchTerm) || u.Email.Contains(request.SearchTerm));
        }

        // Order alphabetically by name
        query = query.OrderBy(u => u.Name);

        // Get total count before pagination
        var totalCount = await query.CountAsync(ct);
        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        // Apply pagination
        var users = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(u => new UserListItemResponse(
                u.Id,
                u.Name,
                u.Email,
                u.Created,
                u.Updated,
                u.UsersRoles.Select(ur => new UserCompanyRoleResponse(
                    ur.CompanyId,
                    ur.CompanyModel.Name,
                    ur.RoleId,
                    ur.RoleModel.Name
                )).ToList()
            ))
            .ToListAsync(ct);

        return new ListUsersResponse(
            request.Page,
            request.PageSize,
            totalCount,
            totalPages,
            request.Page > 1,
            request.Page < totalPages,
            users
        );
    }
}
