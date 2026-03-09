using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.User.UpdateUser;

public class UpdateUserHandler(DefaultContext context)
{
    public virtual async Task<UpdateUserResponse> Handle(UpdateUserQuery request, CancellationToken ct)
    {
        var user = await context.AuthUsers
            .FirstOrDefaultAsync(u => u.Id == request.UserId, ct) ?? throw new InvalidRequestException("User not found");

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            user.Name = request.Name;
        }

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var emailExists = await context.AuthUsers
                .AnyAsync(u => u.Email == request.Email && u.Id != request.UserId, ct);

            if (emailExists)
            {
                throw new InvalidRequestException("This email already exists");
            }

            user.Email = request.Email;
        }

        user.Updated = DateTime.UtcNow;

        if (request.CompaniesRoles != null && request.CompaniesRoles.Any())
        {
            var existingRoles = await context.UserRoles
                .Where(ur => ur.UserId == request.UserId)
                .ToListAsync(ct);

            var requestedPairs = request.CompaniesRoles
                .Select(cr => (cr.CompanyId, cr.RoleId))
                .ToHashSet();

            var rolesToRemove = existingRoles
                .Where(er => !requestedPairs.Contains((er.CompanyId, er.RoleId)))
                .ToList();
            context.UserRoles.RemoveRange(rolesToRemove);

            var existingPairs = existingRoles
                .Select(er => (er.CompanyId, er.RoleId))
                .ToHashSet();

            foreach (var companyRole in request.CompaniesRoles.Where(companyRole => !existingPairs.Contains((companyRole.CompanyId, companyRole.RoleId))))
            {
                var company = await context.Companies.FindAsync([companyRole.CompanyId, ct], cancellationToken: ct) ?? throw new InvalidRequestException($"Company with ID {companyRole.CompanyId} not found");
                var role = await context.Roles.FindAsync([companyRole.RoleId, ct], cancellationToken: ct) ?? throw new InvalidRequestException($"Role with ID {companyRole.RoleId} not found");

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

        var updatedUser = await context.AuthUsers
            .Include(u => u.UsersRoles)
                .ThenInclude(ur => ur.Role)
            .Include(u => u.UsersRoles)
                .ThenInclude(ur => ur.Company)
            .FirstOrDefaultAsync(u => u.Id == user.Id, ct);

        var companiesRolesResponse = updatedUser!.UsersRoles.Select(ur =>
            new UserCompanyRoleResponse(
                ur.CompanyId,
                ur.Company.Name,
                ur.RoleId,
                ur.Role.Name
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
