using Fenicia.Common;
using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Company.GetCompaniesByUser;

public sealed class GetCompaniesByUserHandler(AuthContext db)
{

    public async Task<Pagination<IEnumerable<CompanyListItemResponse>>> Handle(
        GetCompaniesByUserQuery query,
        CancellationToken ct)
    {
        var baseQuery = db.UserRoles.Where(ur => ur.UserId == query.UserId && ur.Company.IsActive);
        var total = await baseQuery.CountAsync(ct);
        var items = await baseQuery
            .OrderBy(ur => ur.Company.Name)
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .Select(CompanyListItemResponse.Projection)
            .ToListAsync(ct);
        
        return new Pagination<IEnumerable<CompanyListItemResponse>>(items, total, query.Page, query.PerPage);


    }
}
