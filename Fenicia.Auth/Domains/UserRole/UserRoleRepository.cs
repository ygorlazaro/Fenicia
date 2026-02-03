using Fenicia.Common.Database.Contexts;

using Fenicia.Common.Database.Models.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.UserRole;

public class UserRoleRepository(AuthContext context) : IUserRoleRepository
{
    public async Task<string[]> GetRolesByUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await context.UserRoles.Where(ur => ur.UserId == userId).Select(ur => ur.Role.Name).ToArrayAsync(cancellationToken);
    }

    public async Task<bool> ExistsInCompanyAsync(Guid userId, Guid companyId, CancellationToken cancellationToken)
    {
        return await context.UserRoles.AnyAsync(ur => ur.UserId == userId && ur.CompanyId == companyId, cancellationToken);
    }

    public async Task<bool> HasRoleAsync(Guid guid, Guid companyId, string role, CancellationToken cancellationToken)
    {
        return await context.UserRoles.AnyAsync(ur => ur.UserId == guid && ur.CompanyId == companyId && ur.Role.Name == role, cancellationToken);
    }

    public Task<List<UserRoleModel>> GetUserCompaniesAsync(Guid userId, CancellationToken cancellationToken)
    {
        var query = from userRole in context.UserRoles
                    join company in context.Companies on userRole.CompanyId equals company.Id
                    where userRole.UserId == userId
                    select new UserRoleModel
                    {
                        Id = company.Id,
                        Role = userRole.Role,
                        Company = new CompanyModel
                        {
                            Id = company.Id,
                            Name = company.Name,
                            Cnpj = company.Cnpj
                        }
                    };

        return query.ToListAsync(cancellationToken);
    }

    public void Add(UserRoleModel userRole)
    {
        context.Add(userRole);
    }
}
