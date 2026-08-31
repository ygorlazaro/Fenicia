using Fenicia.Auth.Domains.UserRole.DTOs;
using Fenicia.Common.Data.Models.Auth;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.UserRole;

public class UserRoleService(UserRoleRepository userRoleRepository)
{
    public async Task<List<UserRoleResponse>> GetCompaniesByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var userRoles = await userRoleRepository.GetCompaniesByUserAsync(userId, cancellationToken);

        return [.. userRoles.Select(ur => ur.MapToUserRoleResponse())];
    }

    public async Task<List<GetUserCompaniesResponse>> GetUserCompaniesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var userRoles = await userRoleRepository.GetUserCompaniesAsync(userId, cancellationToken);

        return [.. userRoles.Select(ur => ur.MapToGetUserCompaniesResponse())];
    }

    public async Task<List<UserRoleModel>> GetUserRolesAsync(Guid userId, int page, int perPage, CancellationToken cancellationToken = default)
    {
        return await userRoleRepository.GetUserRolesAsync(userId, page, perPage, cancellationToken);
    }

    public async Task<int> CountUserRolesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await userRoleRepository.CountUserRolesAsync(userId, cancellationToken);
    }

    public async Task<UserRoleModel?> GetUserRoleAsync(Guid userId, Guid companyId, CancellationToken cancellationToken = default)
    {
        return await userRoleRepository.GetUserRoleAsync(userId, companyId, cancellationToken);
    }

    public async Task<bool> IsAdminAsync(Guid userId, Guid companyId, CancellationToken cancellationToken = default)
    {
        return await userRoleRepository.IsAdminAsync(userId, companyId, cancellationToken);
    }

    public async Task<bool> AnyIdAndCompanyAsync(Guid userId, Guid companyId, CancellationToken cancellationToken = default)
    {
        return await userRoleRepository.AnyIdAndCompanyAsync(userId, companyId, cancellationToken);
    }

    public async Task<bool> HasRoleAsync(Guid userId, Guid companyId, string role, CancellationToken cancellationToken = default)
    {
        return await userRoleRepository.HasRoleAsync(userId, companyId, role, cancellationToken);
    }

    public async Task InsertRangeAsync(List<UserRoleModel> userRoles, CancellationToken cancellationToken = default)
    {
        await userRoleRepository.InsertRangeAsync(userRoles, cancellationToken);
    }

    public async Task<UserRoleModel> InsertAsync(UserRoleModel userRole, CancellationToken cancellationToken = default)
    {
        return await userRoleRepository.InsertAsync(userRole, cancellationToken);
    }

    public async Task DeleteAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        await userRoleRepository.DeleteAsync(roleId, cancellationToken);
    }

    public async Task<List<UserRoleModel>> GetUserRolesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await userRoleRepository.Query()
            .Include(x => x.Role)
            .Where(x => x.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<UserRoleModel>> GetUserRoleModelsByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await userRoleRepository.GetCompaniesByUserAsync(userId, cancellationToken);
    }
}
