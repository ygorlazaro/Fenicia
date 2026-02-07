using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.UserRole;

public class UserRoleRepository(AuthContext context) : IUserRoleRepository
{
    public async Task<string[]> GetRolesByUserAsync(Guid userId, CancellationToken ct)
    {
        return await context.UserRoles.Where(ur => ur.UserId == userId).Select(ur => ur.Role.Name).ToArrayAsync(ct);
    }

    public async Task<bool> ExistsInCompanyAsync(Guid userId, Guid companyId, CancellationToken ct)
    {
        return await context.UserRoles.AnyAsync(ur => ur.UserId == userId && ur.CompanyId == companyId, ct);
    }

    public async Task<bool> HasRoleAsync(Guid guid, Guid companyId, string role, CancellationToken ct)
    {
        return await context.UserRoles.AnyAsync(
            ur => ur.UserId == guid && ur.CompanyId == companyId && ur.Role.Name == role, ct);
    }

    public Task<List<UserRoleModel>> GetUserCompaniesAsync(Guid userId, CancellationToken ct)
    {
        var query = from ur in context.UserRoles
                    join c in context.Companies on ur.CompanyId equals c.Id
                    where ur.UserId == userId
                    select new UserRoleModel
                    {
                        Id = c.Id,
                        Role = ur.Role,
                        Company = new CompanyModel
                        {
                            Id = c.Id,
                            Name = c.Name,
                            Cnpj = c.Cnpj
                        }
                    };

        return query.ToListAsync(ct);
    }

    public void Add(UserRoleModel userRole)
    {
        context.Add(userRole);
    }
}