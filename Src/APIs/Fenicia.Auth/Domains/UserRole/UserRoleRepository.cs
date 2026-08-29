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
        var query = from ur in DbSet
                    where ur.UserId == userId && ur.Deleted == null
                    select ur;

        var userRoles = await query.ToListAsync(ct);

        return userRoles.Select(ur => ur.MapToUserRoleResponse()).ToList();
    }

    public async Task<List<GetUserCompaniesResponse>> GetUserCompaniesAsync(Guid userId, CancellationToken ct = default)
    {
        var query = from ur in DbSet
                    where ur.UserId == userId && ur.Deleted == null
                    select ur;

        var userRoles = await query.ToListAsync(ct);

        return userRoles.Select(ur => ur.MapToGetUserCompaniesResponse()).ToList();
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
