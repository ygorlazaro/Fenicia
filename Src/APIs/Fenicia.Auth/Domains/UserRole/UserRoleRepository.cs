using Fenicia.Auth.Domains.UserRole.Interfaces;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.UserRole;

public class UserRoleRepository(DefaultContext context) : Repository<UserRoleModel>(context), IUserRoleRepository
{
    public async Task<List<UserRoleModel>> GetCompaniesByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(ur => ur.UserId == userId)
            .Include(ur => ur.Role)
            .Include(ur => ur.Company)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<UserRoleModel>> GetUserCompaniesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(ur => ur.UserId == userId)
            .Include(ur => ur.Role)
            .Include(ur => ur.Company)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<UserRoleModel>> GetUserRolesAsync(Guid userId, int page, int perPage, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(ur => ur.UserId == userId && ur.Company.IsActive)
            .Include(ur => ur.Role)
            .Include(ur => ur.Company)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountUserRolesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbSet.CountAsync(ur => ur.UserId == userId && ur.Company.IsActive, cancellationToken);
    }

    public async Task<UserRoleModel?> GetUserRoleAsync(Guid userId, Guid companyId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.CompanyId == companyId, cancellationToken);
    }

    public async Task<bool> IsAdminAsync(Guid userId, Guid companyId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AnyAsync(ur => ur.UserId == userId && ur.CompanyId == companyId && ur.Role.Name == "Admin", cancellationToken);
    }

    public async Task<bool> AnyIdAndCompanyAsync(Guid userId, Guid companyId, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(ur => ur.UserId == userId && ur.CompanyId == companyId, cancellationToken);
    }

    public async Task<bool> HasRoleAsync(Guid userId, Guid companyId, string role, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(ur => ur.UserId == userId && ur.CompanyId == companyId && ur.Role.Name == role, cancellationToken);
    }
}
