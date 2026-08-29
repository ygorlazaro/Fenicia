using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;
using Fenicia.Common.Enums.Auth;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Company;

public class CompanyRepository(DefaultContext context) : Repository<CompanyModel>(context)
{
    public async Task<CompanyModel?> GetByCnpjAsync(string cnpj, CancellationToken ct = default)
    {
        return await DbSet.FirstOrDefaultAsync(c => c.Cnpj == cnpj && c.Deleted == null, ct);
    }

    public async Task<CompanyModel?> AnyActiveAsync(Guid companyId, CancellationToken ct = default)
    {
        return await DbSet.FirstOrDefaultAsync(c => c.Id == companyId && c.IsActive, ct);
    }

    public async Task<bool> AnyAsync(Guid companyId, CancellationToken ct = default)
    {
        return await DbSet.AnyAsync(c => c.Id == companyId, ct);
    }

    public async Task<bool> CheckExistsAsync(string cnpj, bool onlyActive, CancellationToken ct = default)
    {
        return await DbSet.AnyAsync(c => c.Cnpj == cnpj && (!onlyActive || c.IsActive), ct);
    }

    public async Task<List<UserRoleModel>> GetUserRolesAsync(Guid userId, int page, int perPage, CancellationToken ct = default)
    {
        var query = from ur in Context.AuthUserRoles
                    where ur.UserId == userId && ur.Company.IsActive
                    select ur;

        return await query.Skip((page - 1) * perPage).Take(perPage).ToListAsync(ct);
    }

    public async Task<int> CountUserRolesAsync(Guid userId, CancellationToken ct = default)
    {
        return await Context.AuthUserRoles.CountAsync(ur => ur.UserId == userId && ur.Company.IsActive, ct);
    }

    public async Task<UserRoleModel?> GetUserRoleAsync(Guid userId, Guid companyId, CancellationToken ct = default)
    {
        return await Context.AuthUserRoles
            .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.CompanyId == companyId, ct);
    }

    public async Task<bool> IsAdminAsync(Guid userId, Guid companyId, CancellationToken ct = default)
    {
        return await Context.AuthUserRoles
            .AnyAsync(ur => ur.UserId == userId && ur.CompanyId == companyId && ur.Role.Name == "Admin", ct);
    }
}
