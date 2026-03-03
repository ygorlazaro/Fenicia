using Fenicia.Common;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Exceptions;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Company.GetCompaniesByUser;

public sealed class GetCompaniesByUserHandler(DefaultContext db)
{
    public async Task<Pagination<IEnumerable<GetCompaniesByUserResponse>>> Handle(
        GetCompaniesByUserQuery query,
        CancellationToken ct)
    {
        if (query.PerPage <= 0)
        {
            throw new ItemNotExistsException(TextConstants.ThereWasAnErrorSearchingModulesMessage);
        }

        var baseQuery = db.UserRoles.Where(ur => ur.UserId == query.UserId && ur.CompanyModel.IsActive);
        var total = await baseQuery.CountAsync(ct);
        var items = await baseQuery
            .OrderBy(ur => ur.CompanyModel.Name)
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .Select(ur => new GetCompaniesByUserResponse(ur.CompanyModel.Id, ur.CompanyModel.Name, ur.CompanyModel.Cnpj, ur.RoleModel.Name))
            .ToListAsync(ct);

        return new Pagination<IEnumerable<GetCompaniesByUserResponse>>(items, total, query.Page, query.PerPage);
    }
}
