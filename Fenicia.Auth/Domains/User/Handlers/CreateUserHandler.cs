using Fenicia.Auth.Domains.Security;
using Fenicia.Auth.Domains.User.Commands;
using Fenicia.Auth.Domains.User.Responses;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.User.Handlers;

public class CreateUserHandler(DefaultContext db) : IRequestHandler<CreateUserCommand, CreateUserResponse>
{
    public virtual async Task<CreateUserResponse> Handle(CreateUserCommand command, CancellationToken ct)
    {
        var userExists = await db.AuthUsers.AnyEmailAsync(command.Email, ct);

        if (userExists)
        {
            throw new InvalidRequestException(ExceptionMessages.EmailAlreadyExists);
        }

        var hashedPassword = command.Password.Hash();

        var user = new UserModel
        {
            Email = command.Email,
            Password = hashedPassword,
            Name = command.Name
        };

        db.AuthUsers.Add(user);
        await RelateRolesAsync(user.Id, command.Roles, ct);
        await db.SaveChangesAsync(ct);

        return new CreateUserResponse(user.Id, user.Name, user.Email);
    }

    private async Task RelateRolesAsync(Guid userId, List<CreateUserRoleCommand>? command, CancellationToken ct)
    {
        var roles = command ?? [];
        await ValidateCompanies(roles.Select(r => r.CompanyId), ct);
        await ValidateRoles(roles.Select(r => r.RoleId), ct);

        var userRoles = roles.Select(r => new UserRoleModel
        {
            UserId = userId,
            RoleId = r.RoleId,
            CompanyId = r.CompanyId
        });

        db.AuthUserRoles.AddRange(userRoles);
    }

    private async Task ValidateCompanies(IEnumerable<Guid> companies, CancellationToken ct)
    {
        var distinct = companies.Distinct();

        var query = db.AuthCompanies.Where(c => distinct.Contains(c.Id));

        if (distinct.Count() != await query.CountAsync(ct))
        {
            throw new InvalidRequestException(ExceptionMessages.CompanyNotFoundMessage);
        }
    }

    private async Task ValidateRoles(IEnumerable<Guid> roles, CancellationToken ct)
    {
        var distinct = roles.Distinct();

        var query = db.AuthRoles.Where(r => distinct.Contains(r.Id));

        if (distinct.Count() != await query.CountAsync(ct))
        {
            throw new InvalidRequestException(ExceptionMessages.RoleNotFound);
        }
    }
}
