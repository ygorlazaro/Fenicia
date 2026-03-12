using Fenicia.Auth.Domains.Company;
using Fenicia.Auth.Domains.User.Commands;
using Fenicia.Auth.Domains.User.Responses;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.User.Handlers;

public class UpdateUserHandler(
    DefaultContext db)
{
    public virtual async Task<UpdateUserResponse> Handle(UpdateUserCommand command, CancellationToken ct)
    {
        var user = await db.AuthUsers.FirstByIdAsync(command.UserId,
            ct);

        await ValidateFields(user,
            command,
            ct);
        
        var companies = command.CompaniesRoles?.Select(c => c.CompanyId) ?? [];
        
        await ValidateCompanies(companies,
            ct);
        await RelateRolesAsync(command,
            user,
            ct);
        await db.SaveChangesAsync(ct);

        return new UpdateUserResponse(
            user.Id,
            user.Name,
            user.Email
        );
    }

    private async Task ValidateCompanies(IEnumerable<Guid> companies, CancellationToken ct)
    {
        foreach (var companyId in companies)
        {
            await db.AuthCompanies.ValidateExistingAsync(companyId,
                ct);
        }
    }

    private async Task RelateRolesAsync(UpdateUserCommand command, UserModel user, CancellationToken ct)
    {
        var requestedRoles = command.CompaniesRoles ?? [];

        if (requestedRoles.Count == 0)
        {
            var existing = await db.AuthUserRoles
                .Where(x => x.UserId == user.Id)
                .ToListAsync(ct);

            db.AuthUserRoles.RemoveRange(existing);
            return;
        }

        var requestedRoleIds = requestedRoles
            .Select(r => r.RoleId)
            .Distinct()
            .ToList();

        var validRoleIds = await db.AuthRoles
            .Where(r => requestedRoleIds.Contains(r.Id))
            .Select(r => r.Id)
            .ToListAsync(ct);

        if (validRoleIds.Count != requestedRoleIds.Count)
        {
            var missingRoles = requestedRoleIds.Except(validRoleIds);

            throw new InvalidRequestException(
                $"Role(s) not found: {string.Join(", ", missingRoles)}"
            );
        }

        var requestedSet = requestedRoles
            .Select(r => (r.CompanyId, r.RoleId))
            .ToHashSet();

        var existingRoles = await db.AuthUserRoles
            .Where(x => x.UserId == user.Id)
            .ToListAsync(ct);

        var existingSet = existingRoles
            .Select(r => (r.CompanyId, r.RoleId))
            .ToHashSet();

        var toRemove = existingRoles
            .Where(r => !requestedSet.Contains((r.CompanyId, r.RoleId)))
            .ToList();

        var toInsert = requestedRoles
            .Where(r => !existingSet.Contains((r.CompanyId, r.RoleId)))
            .Select(r => new UserRoleModel
            {
                UserId = user.Id,
                CompanyId = r.CompanyId,
                RoleId = r.RoleId
            })
            .ToList();

        if (toRemove.Count > 0)
        {
            db.AuthUserRoles.RemoveRange(toRemove);
        }

        if (toInsert.Count > 0)
        {
            db.AuthUserRoles.AddRange(toInsert);
        }
    }

    private async Task ValidateFields(UserModel user, UpdateUserCommand command, CancellationToken ct)
    {
        user.Name = string.IsNullOrWhiteSpace(command.Name) switch
        {
            false => command.Name,
            _ => user.Name
        };

        if (!string.IsNullOrWhiteSpace(command.Email))
        {
            var emailExists = await db.AuthUsers
                .AnyAsync(u => u.Email == command.Email && u.Id != command.UserId,
                    ct);

            user.Email = emailExists switch
            {
                true => throw new InvalidRequestException(ExceptionMessages.EmailAlreadyExists),
                _ => command.Email
            };

        }
    }
}
