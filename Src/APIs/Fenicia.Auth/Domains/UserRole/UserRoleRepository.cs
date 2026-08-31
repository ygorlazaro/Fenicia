using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.UserRole;

public class UserRoleRepository(DefaultContext context) : Repository<UserRoleModel>(context)
{
    public async Task<List<UserRoleModel>> GetCompaniesByUserAsync(Guid userId, CancellationToken ct)
    {
        return await DbSet
            .Where(ur => ur.UserId == userId && ur.Deleted == null)
            .Include(ur => ur.Role)
            .Include(ur => ur.Company)
            .ToListAsync(ct);
    }

    public async Task<List<UserRoleModel>> GetUserCompaniesAsync(Guid userId, CancellationToken ct)
    {
        return await DbSet
            .Where(ur => ur.UserId == userId && ur.Deleted == null)
            .Include(ur => ur.Role)
            .Include(ur => ur.Company)
            .ToListAsync(ct);
    }

    public async Task<List<UserRoleModel>> GetUserRolesAsync(Guid userId, int page, int perPage, CancellationToken ct)
    {
        return await DbSet
            .Where(ur => ur.UserId == userId && ur.Deleted == null && ur.Company.IsActive)
            .Include(ur => ur.Role)
            .Include(ur => ur.Company)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync(ct);
    }

    public async Task<int> CountUserRolesAsync(Guid userId, CancellationToken ct)
    {
        return await DbSet.CountAsync(ur => ur.UserId == userId && ur.Deleted == null && ur.Company.IsActive, ct);
    }

    public async Task<UserRoleModel?> GetUserRoleAsync(Guid userId, Guid companyId, CancellationToken ct)
    {
        return await DbSet
            .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.CompanyId == companyId && ur.Deleted == null, ct);
    }

    public async Task<bool> IsAdminAsync(Guid userId, Guid companyId, CancellationToken ct)
    {
        return await DbSet
            .AnyAsync(ur => ur.UserId == userId && ur.CompanyId == companyId && ur.Role.Name == "Admin" && ur.Deleted == null, ct);
    }

    public async Task<bool> AnyIdAndCompanyAsync(Guid userId, Guid companyId, CancellationToken ct)
    {
        return await DbSet.AnyAsync(ur => ur.UserId == userId && ur.CompanyId == companyId && ur.Deleted == null, ct);
    }

    public async Task<bool> HasRoleAsync(Guid userId, Guid companyId, string role, CancellationToken ct)
    {
        return await DbSet.AnyAsync(ur => ur.UserId == userId && ur.CompanyId == companyId && ur.Role.Name == role && ur.Deleted == null, ct);
    }
}
