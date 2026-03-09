using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Company.UpdateCompany;

public sealed class UpdateCompanyHandler(DefaultContext context)
{
    public async Task Handle(UpdateCompanyCommand command, CancellationToken ct)
    {
        var company = await context.AuthCompanies
                          .FirstOrDefaultAsync(c => c.Id == command.CompanyId && c.IsActive, ct)
                      ?? throw new ItemNotExistsException(ExceptionMessages.CompanyNotFoundMessage);

        var isAdmin = await HasRoleAsync(
            command.UserId,
            command.CompanyId,
            "Admin",
            ct
        );

        if (!isAdmin)
        {
            throw new PermissionDeniedException(ExceptionMessages.PermissionDeniedUpdateCompany);
        }

        company.Name = command.Name;
        company.TimeZone = command.TimeZone;

        await context.SaveChangesAsync(ct);
    }

    private async Task<bool> HasRoleAsync(Guid userId, Guid companyId, string role, CancellationToken ct)
    {
        var query = context.AuthUserRoles.Where(ur => ur.UserId == userId
                                                  && ur.CompanyId == companyId && ur.Role.Name == role)
            .Select(ur => 1);

        return await query.AnyAsync(ct);
    }
}
