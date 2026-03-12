using Fenicia.Auth.Domains.Company.Queries;
using Fenicia.Auth.Domains.Company.Responses;
using Fenicia.Common;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Company.Handlers;

public sealed class GetCompaniesByUserHandler(DefaultContext db)
{
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

        return new Pagination<IEnumerable<GetCompaniesByUserResponse>>(items,
            total,
            query.Page,
            query.PerPage);
    }
}
