using Fenicia.Auth.Domains.UserRole.DTOs;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.UserRole;

public class UserRoleRepository(DefaultContext context) : Repository<UserRoleModel>(context)
{
    public async Task<List<UserRoleResponse>> GetCompaniesByUserAsync(Guid userId, CancellationToken ct = default)
    {
        return await DbSet
                .Where(ur => ur.UserId == userId && ur.Deleted == null)
                .Select(ur => new UserRoleResponse(ur.CompanyId, ur.Role.Name, new CompanyResponse(ur.CompanyId, ur.Company.Name, ur.Company.Cnpj)))
                .ToListAsync(ct);
    }

    public async Task<List<GetUserCompaniesResponse>> GetUserCompaniesAsync(Guid userId, CancellationToken ct = default)
    {
        return await DbSet
                .Where(ur => ur.UserId == userId && ur.Deleted == null)
                .Select(ur => new GetUserCompaniesResponse(ur.CompanyId, ur.Role.Name, ur.CompanyId, ur.Company.Name, ur.Company.Cnpj))
                .ToListAsync(ct);
    }

    public async Task<bool> AnyIdAndCompanyAsync(Guid userId, Guid companyId, CancellationToken ct = default)
    {
        return await DbSet.AnyAsync(ur => ur.UserId == userId && ur.CompanyId == companyId && ur.Deleted == null, ct);
    }

    public async Task<bool> HasRoleAsync(Guid userId, Guid companyId, string role, CancellationToken ct = default)
    {
        return await DbSet.AnyAsync(ur => ur.UserId == userId && ur.CompanyId == companyId && ur.Role.Name == role && ur.Deleted == null, ct);
    }
}
