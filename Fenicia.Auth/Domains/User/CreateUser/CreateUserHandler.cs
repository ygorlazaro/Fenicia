using Fenicia.Auth.Domains.Security.HashPassword;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.User.CreateUser;

public class CreateUserHandler(
    DefaultContext context,
    CheckUserExistsHandler checkUserExistsHandler,
    HashPasswordHandler hashPasswordHandler)
{
    public virtual async Task<CreateUserResponse> Handle(CreateUserQuery request, CancellationToken ct)
    {
        var userExists = await checkUserExistsHandler.Handle(request.Email, ct);
        if (userExists)
        {
            throw new InvalidRequestException("This email already exists");
        }

        var hashedPassword = hashPasswordHandler.Handle(request.Password);

        var user = new UserModel
        {
            Email = request.Email,
            Password = hashedPassword,
            Name = request.Name
        };

        context.AuthUsers.Add(user);

        if (request.CompaniesRoles != null && request.CompaniesRoles.Any())
        {
            foreach (var companyRole in request.CompaniesRoles)
            {
                var company = await context.Companies.FindAsync([companyRole.CompanyId, ct], ct) ?? throw new InvalidRequestException($"Company with ID {companyRole.CompanyId} not found");
                var role = await context.Roles.FindAsync([companyRole.RoleId, ct], ct) ?? throw new InvalidRequestException($"Role with ID {companyRole.RoleId} not found");

                var userRole = new UserRoleModel
                {
                    UserId = user.Id,
                    CompanyId = companyRole.CompanyId,
                    RoleId = companyRole.RoleId
                };

                context.UserRoles.Add(userRole);
            }
        }

        await context.SaveChangesAsync(ct);

        var userWithRelations = await context.AuthUsers
            .Include(u => u.UsersRoles)
                .ThenInclude(ur => ur.Role)
            .Include(u => u.UsersRoles)
                .ThenInclude(ur => ur.Company)
            .FirstOrDefaultAsync(u => u.Id == user.Id, ct);

        var companiesRolesResponse = userWithRelations!.UsersRoles.Select(ur => 
            new UserCompanyRoleResponse(
                ur.CompanyId,
                ur.Company.Name,
                ur.RoleId,
                ur.Role.Name
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
