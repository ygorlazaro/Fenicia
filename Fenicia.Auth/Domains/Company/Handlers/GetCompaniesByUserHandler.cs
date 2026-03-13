using Fenicia.Auth.Domains.Company.Queries;
using Fenicia.Auth.Domains.Company.Responses;
using Fenicia.Common;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Company.Handlers;

/// <summary>
/// Handler responsible for retrieving companies associated with a specific user.
/// Returns a paginated list of companies where the user has a role, ordered by company name.
/// </summary>
public sealed class GetCompaniesByUserHandler(DefaultContext db)
{
    /// <summary>
    /// Retrieves paginated companies for a user.
    /// </summary>
    /// <param name="query">The query containing user ID, page number, and items per page.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A paginated response containing companies the user is associated with.</returns>
    /// <exception cref="InvalidRequestException">Thrown when the perPage parameter is zero or negative.</exception>
    public async Task<Pagination<IEnumerable<GetCompaniesByUserResponse>>> Handle(
        GetCompaniesByUserQuery query,
        CancellationToken ct)
    {
        if (query.PerPage <= 0)
        {
            throw new InvalidRequestException(ExceptionMessages.UserNotAssociatedWithActiveCompanies);
        }

        var request = db.AuthUserRoles.Where(ur => ur.UserId == query.UserId && ur.Company.IsActive);
        var total = await request.CountAsync(ct);
        var items = await request
            .OrderBy(ur => ur.Company.Name)
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .Select(ur => new GetCompaniesByUserResponse(ur.Company.Id,
                ur.Company.Name,
                ur.Company.Cnpj,
                ur.Role.Name))
            .ToListAsync(ct);

        return new Pagination<IEnumerable<GetCompaniesByUserResponse>>(items, total, query.Page, query.PerPage);
    }
}
