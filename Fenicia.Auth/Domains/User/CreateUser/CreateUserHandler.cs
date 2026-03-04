using Fenicia.Auth.Domains.Security.HashPassword;
using Fenicia.Auth.Domains.User.CreateUser;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.User.CreateUser;

public class CreateUserHandler(
    DefaultContext context,
    CheckUserExistsHandler checkUserExistsHandler,
    HashPasswordHandler hashPasswordHandler)
{
    public virtual async Task<CreateUserResponse> Handle(CreateUserQuery request, CancellationToken ct)
    {
        // Check if user already exists
        var userExists = await checkUserExistsHandler.Handle(request.Email, ct);
        if (userExists)
        {
            throw new ArgumentException("This email already exists");
        }

        // Hash password
        var hashedPassword = hashPasswordHandler.Handle(request.Password);

        // Create user
        var user = new AuthUserModel
        {
            Email = request.Email,
            Password = hashedPassword,
            Name = request.Name
        };

        context.AuthUsers.Add(user);

        // Add company roles if provided
        if (request.CompaniesRoles != null && request.CompaniesRoles.Any())
        {
            foreach (var companyRole in request.CompaniesRoles)
            {
                // Verify company exists
                var company = await context.Companies.FindAsync(companyRole.CompanyId, ct);
                if (company == null)
                {
                    throw new ArgumentException($"Company with ID {companyRole.CompanyId} not found");
                }

                // Verify role exists
                var role = await context.Companies.FindAsync(companyRole.RoleId, ct);
                if (role == null)
                {
                    throw new ArgumentException($"Role with ID {companyRole.RoleId} not found");
                }

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

        // Load company and role names for response
        var userWithRelations = await context.AuthUsers
            .Include(u => u.UsersRoles)
                .ThenInclude(ur => ur.RoleModel)
            .Include(u => u.UsersRoles)
                .ThenInclude(ur => ur.CompanyModel)
            .FirstOrDefaultAsync(u => u.Id == user.Id, ct);

        var companiesRolesResponse = userWithRelations!.UsersRoles.Select(ur => 
            new UserCompanyRoleResponse(
                ur.CompanyId,
                ur.CompanyModel.Name,
                ur.RoleId,
                ur.RoleModel.Name
            )
        ).ToList();

        return new CreateUserResponse(
            user.Id,
            user.Name,
            user.Email,
            user.Created,
            companiesRolesResponse
        );
    }
}
