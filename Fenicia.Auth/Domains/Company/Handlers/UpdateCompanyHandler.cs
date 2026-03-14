using Fenicia.Auth.Domains.Company.Commands;
using Fenicia.Auth.Domains.UserRole;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;

namespace Fenicia.Auth.Domains.Company.Handlers;

/// <summary>
///     Handler responsible for processing company update requests.
///     Validates that the company exists, is active, and that the user has Admin permissions
///     before applying the update.
/// </summary>
public sealed class UpdateCompanyHandler(DefaultContext db)
{
    /// <summary>
    ///     Handles the company update operation.
    /// </summary>
    /// <param name="command">The update command containing company ID, user ID, and new name.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ItemNotExistsException">Thrown when the company does not exist or is inactive.</exception>
    /// <exception cref="PermissionDeniedException">Thrown when the user does not have Admin permissions.</exception>
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