using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.UserRole.GetCompaniesByUser;

public class GetCompaniesByUserHandler(DefaultContext context)
{
    public async Task<List<UserRoleResponse>> GetUserCompaniesAsync(
        GetCompaniesByUserQuery request,
        CancellationToken ct)
    {
        var query = from ur in context.UserRoles
                    join c in context.Companies on ur.CompanyId equals c.Id
                    where ur.UserId == request.UserId
                    let company = new CompanyResponse(c.Id, c.Name, c.Cnpj)
                    select new UserRoleResponse(c.Id, ur.RoleModel.Name, company);

        return await query.ToListAsync(ct);
    }
}
