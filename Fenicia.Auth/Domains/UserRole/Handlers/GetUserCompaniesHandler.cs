using Fenicia.Auth.Domains.UserRole.Responses;
using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.UserRole.Handlers;

public class GetUserCompaniesHandler(DefaultContext db)
{
    public async Task<List<GetUserCompaniesResponse>> Handle(Guid userId, CancellationToken ct)
    {
        var query = from ur in db.AuthUserRoles
            join c in db.AuthCompanies on ur.CompanyId equals c.Id
            where ur.UserId == userId
            select new GetUserCompaniesResponse(c.Id,
                ur.Role.Name,
                c.Id,
                c.Name,
                c.Cnpj);

        return await query.ToListAsync(ct);
    }
}