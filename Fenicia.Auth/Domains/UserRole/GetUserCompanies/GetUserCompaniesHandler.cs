using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.UserRole.GetUserCompanies;

public class GetUserCompaniesHandler(AuthContext context)
{
    public async Task<List<GetUserCompaniesResponse>> Handle(
        Guid userId,
        CancellationToken ct)
    {
        var query = from ur in context.UserRoles
                    join c in context.Companies on ur.CompanyId equals c.Id
                    where ur.UserId == userId
                    select new GetUserCompaniesResponse(c.Id, ur.Role.Name, new CompanyResponse(c.Id, c.Name, c.Cnpj));

        return await query.ToListAsync(ct);
    }
}