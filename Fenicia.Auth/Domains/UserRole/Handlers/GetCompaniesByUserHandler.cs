using Fenicia.Auth.Domains.UserRole.Queries;
using Fenicia.Auth.Domains.UserRole.Responses;
using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.UserRole.Handlers;

public class GetCompaniesByUserHandler(DefaultContext db)
{
    public async Task<List<UserRoleResponse>> GetUserCompaniesAsync(GetCompaniesByUserQuery request, CancellationToken ct)
    {
        var query = from ur in db.AuthUserRoles
            join c in db.AuthCompanies on ur.CompanyId equals c.Id
            where ur.UserId == request.UserId
            let company = new CompanyResponse(c.Id,
                c.Name,
                c.Cnpj)
            select new UserRoleResponse(c.Id,
                ur.Role.Name,
                company);

        return await query.ToListAsync(ct);
    }
}