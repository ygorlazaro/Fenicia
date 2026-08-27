using Fenicia.Auth.Domains.Company.Commands;
using Fenicia.Auth.Domains.UserRole;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;

using MediatR;

namespace Fenicia.Auth.Domains.Company.Handlers;

public sealed class UpdateCompanyHandler(DefaultContext db) : IRequestHandler<UpdateCompanyCommand>
{

    public async Task Handle(UpdateCompanyCommand command, CancellationToken ct)
    {
        var company = await db.AuthCompanies.AnyActiveAsync(command.CompanyId, ct) ?? throw new ItemNotExistsException(ExceptionMessages.CompanyNotFoundMessage);
        var isAdmin = await db.AuthUserRoles.HasRoleAsync(command.UserId, command.CompanyId, "Admin", ct);

        if (!isAdmin)
        {
            throw new PermissionDeniedException(ExceptionMessages.PermissionDeniedUpdateCompany);
        }

        company.Name = command.Name;

        await db.SaveChangesAsync(ct);
    }
}
