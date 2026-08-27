using Fenicia.Auth.Domains.UserRole.DTOs.Responses;
using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.UserRole;

public class UserRoleService(DefaultContext db)
{
    public async Task<List<UserRoleResponse>> GetCompaniesByUserAsync(Guid userId, CancellationToken ct)
    {
        var query = from ur in db.AuthUserRoles
                    join c in db.AuthCompanies on ur.CompanyId equals c.Id
                    where ur.UserId == userId
                    let company = new CompanyResponse(c.Id,
                        c.Name,
                        c.Cnpj)
                    select new UserRoleResponse(c.Id,
                        ur.Role.Name,
                        company);

        return await query.ToListAsync(ct);
    }

    public async Task<List<GetUserCompaniesResponse>> GetUserCompaniesAsync(Guid userId, CancellationToken ct)
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

    public async Task<bool> AnyIdAndCompanyAsync(Guid userId, Guid companyId, CancellationToken ct)
    {
        return await db.AuthUserRoles.AnyAsync(ur => ur.CompanyId == companyId && ur.UserId == userId, ct);
    }

    public async Task<bool> HasRoleAsync(Guid userId, Guid companyId, string role, CancellationToken ct)
    {
        return await db.AuthUserRoles.AnyAsync(ur => ur.UserId == userId && ur.CompanyId == companyId && ur.Role.Name == role, ct);
    }
}
