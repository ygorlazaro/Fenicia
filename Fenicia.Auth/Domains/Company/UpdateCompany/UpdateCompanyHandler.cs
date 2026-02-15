using Fenicia.Auth.Domains.UserRole;
using Fenicia.Common;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Exceptions;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Company.UpdateCompany;

public sealed class UpdateCompanyHandler(AuthContext db, IUserRoleService userRoleService)
{
    public async Task Handle(UpdateCompanyCommand command, CancellationToken ct)
    {
        var company = await db.Companies
                          .FirstOrDefaultAsync(c => c.Id == command.CompanyId && c.IsActive, ct)
                      ?? throw new ItemNotExistsException(TextConstants.ItemNotFoundMessage);

        var isAdmin = await userRoleService.HasRoleAsync(
            command.UserId,
            command.CompanyId,
            "Admin",
            ct
        );

        if (!isAdmin)
        {
            throw new PermissionDeniedException(TextConstants.PermissionDeniedMessage);
        }

        company.Name = command.Name;
        company.TimeZone = command.TimeZone;

        await db.SaveChangesAsync(ct);
    }
}
