using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.User.UpdateUser;

public class UpdateUserHandler(DefaultContext context)
{
    public virtual async Task<UpdateUserResponse> Handle(UpdateUserQuery request, CancellationToken ct)
    {
        // Find user
        var user = await context.AuthUsers
            .Include(u => u.UsersRoles)
                .ThenInclude(ur => ur.RoleModel)
            .Include(u => u.UsersRoles)
                .ThenInclude(ur => ur.CompanyModel)
            .FirstOrDefaultAsync(u => u.Id == request.UserId, ct) ?? throw new ArgumentException("User not found");

        // Update fields if provided
        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            user.Name = request.Name;
        }

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            // Check if new email is already taken by another user
            var emailExists = await context.AuthUsers
                .AnyAsync(u => u.Email == request.Email && u.Id != request.UserId, ct);
            
            if (emailExists)
            {
                throw new ArgumentException("This email already exists");
            }
            
            user.Email = request.Email;
        }

        user.Updated = DateTime.UtcNow;

        // Update company roles if provided
        if (request.CompaniesRoles != null)
        {
            // Remove existing roles
            var existingRoles = user.UsersRoles.ToList();
            context.UserRoles.RemoveRange(existingRoles);

            // Add new roles
            foreach (var companyRole in request.CompaniesRoles)
            {
                // Verify company exists
                var company = await context.UserRoles.FindAsync(companyRole.CompanyId, ct) ?? throw new ArgumentException($"Company with ID {companyRole.CompanyId} not found");

                // Verify role exists
                var role = await context.Roles.FindAsync(companyRole.RoleId, ct) ?? throw new ArgumentException($"Role with ID {companyRole.RoleId} not found");
                var userRole = new AuthUserRoleModel
                {
                    UserId = user.Id,
                    CompanyId = companyRole.CompanyId,
                    RoleId = companyRole.RoleId
                };

                context.UserRoles.Add(userRole);
            }
        }

        await context.SaveChangesAsync(ct);

        // Reload to get updated data
        var updatedUser = await context.AuthUsers
            .Include(u => u.UsersRoles)
                .ThenInclude(ur => ur.RoleModel)
            .Include(u => u.UsersRoles)
                .ThenInclude(ur => ur.CompanyModel)
            .FirstOrDefaultAsync(u => u.Id == user.Id, ct);

        var companiesRolesResponse = updatedUser!.UsersRoles.Select(ur =>
            new UserCompanyRoleResponse(
                ur.CompanyId,
                ur.CompanyModel.Name,
                ur.RoleId,
                ur.RoleModel.Name
            )
        ).ToList();

        return new UpdateUserResponse(
            updatedUser.Id,
            updatedUser.Name,
            updatedUser.Email,
            updatedUser.Created,
            updatedUser.Updated,
            companiesRolesResponse
        );
    }
}
