using Fenicia.Auth.Domains.UserRole.DTOs;
using Fenicia.Common.Data.Models.Auth;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.UserRole;

public class UserRoleService(UserRoleRepository userRoleRepository)
{
    public async Task<List<UserRoleResponse>> GetCompaniesByUserAsync(Guid userId, CancellationToken ct)
    {
        var userRoles = await userRoleRepository.GetCompaniesByUserAsync(userId, ct);

        return userRoles.Select(ur => ur.MapToUserRoleResponse()).ToList();
    }

    public async Task<List<GetUserCompaniesResponse>> GetUserCompaniesAsync(Guid userId, CancellationToken ct)
    {
        var userRoles = await userRoleRepository.GetUserCompaniesAsync(userId, ct);

        return userRoles.Select(ur => ur.MapToGetUserCompaniesResponse()).ToList();
    }

    public async Task<List<UserRoleModel>> GetUserRolesAsync(Guid userId, int page, int perPage, CancellationToken ct)
    {
        return await userRoleRepository.GetUserRolesAsync(userId, page, perPage, ct);
    }

    public async Task<int> CountUserRolesAsync(Guid userId, CancellationToken ct)
    {
        return await userRoleRepository.CountUserRolesAsync(userId, ct);
    }

    public async Task<UserRoleModel?> GetUserRoleAsync(Guid userId, Guid companyId, CancellationToken ct)
    {
        return await userRoleRepository.GetUserRoleAsync(userId, companyId, ct);
    }

    public async Task<bool> IsAdminAsync(Guid userId, Guid companyId, CancellationToken ct)
    {
        return await userRoleRepository.IsAdminAsync(userId, companyId, ct);
    }

    public async Task<bool> AnyIdAndCompanyAsync(Guid userId, Guid companyId, CancellationToken ct)
    {
        return await userRoleRepository.AnyIdAndCompanyAsync(userId, companyId, ct);
    }

    public async Task<bool> HasRoleAsync(Guid userId, Guid companyId, string role, CancellationToken ct)
    {
        return await userRoleRepository.HasRoleAsync(userId, companyId, role, ct);
    }

    public async Task InsertRangeAsync(List<UserRoleModel> userRoles, CancellationToken ct)
    {
        await userRoleRepository.InsertRangeAsync(userRoles, ct);
    }

    public async Task<UserRoleModel> InsertAsync(UserRoleModel userRole, CancellationToken ct)
    {
        return await userRoleRepository.InsertAsync(userRole, ct);
    }

    public async Task DeleteAsync(Guid roleId, CancellationToken ct)
    {
        await userRoleRepository.DeleteAsync(roleId, ct);
    }

    public async Task<List<UserRoleModel>> GetUserRolesByUserIdAsync(Guid userId, CancellationToken ct)
    {
        return await userRoleRepository.Query()
            .Include(x => x.Role)
            .Where(x => x.UserId == userId)
            .ToListAsync(ct);
    }

    public async Task<List<UserRoleModel>> GetUserRoleModelsByUserAsync(Guid userId, CancellationToken ct)
    {
        return await userRoleRepository.GetCompaniesByUserAsync(userId, ct);
    }
}
