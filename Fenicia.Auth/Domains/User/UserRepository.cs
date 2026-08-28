using Fenicia.Auth.Domains.UserRole.DTOs;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.User;

public class UserRepository(DefaultContext context) : Repository<UserModel>(context)
{
    public async Task<UserModel?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        return await DbSet.FirstOrDefaultAsync(u => u.Email == email && u.Deleted == null, ct);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default)
    {
        return await DbSet.AnyAsync(u => u.Email == email && u.Deleted == null, ct);
    }

    public async Task<List<GetUserCompaniesResponse>> GetCompaniesAsync(Guid userId, CancellationToken ct = default)
    {
        return await DbSet
                .Where(u => u.Id == userId && u.Deleted == null)
                .SelectMany(u => u.UsersRoles)
                .Where(ur => ur.Deleted == null)
                .Select(ur => new GetUserCompaniesResponse(ur.CompanyId, ur.Role.Name, ur.Company.Id, ur.Company.Name, ur.Company.Cnpj))
                .ToListAsync(ct);
    }
}
