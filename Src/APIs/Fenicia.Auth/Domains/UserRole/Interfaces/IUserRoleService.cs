using Fenicia.Auth.Domains.UserRole.DTOs;
using Fenicia.Common.Data.Models.Auth;

namespace Fenicia.Auth.Domains.UserRole.Interfaces;

public interface IUserRoleService
{
    Task<List<UserRoleResponse>> GetCompaniesByUserAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<List<GetUserCompaniesResponse>> GetUserCompaniesAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<List<UserRoleModel>> GetUserRolesAsync(Guid userId, int page, int perPage, CancellationToken cancellationToken = default);

    Task<int> CountUserRolesAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<UserRoleModel?> GetUserRoleAsync(Guid userId, Guid companyId, CancellationToken cancellationToken = default);

    Task<bool> IsAdminAsync(Guid userId, Guid companyId, CancellationToken cancellationToken = default);

    Task<bool> AnyIdAndCompanyAsync(Guid userId, Guid companyId, CancellationToken cancellationToken = default);

    Task<bool> HasRoleAsync(Guid userId, Guid companyId, string role, CancellationToken cancellationToken = default);

    Task InsertRangeAsync(List<UserRoleModel> userRoles, CancellationToken cancellationToken = default);

    Task<UserRoleModel> InsertAsync(UserRoleModel userRole, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid roleId, CancellationToken cancellationToken = default);

    Task<List<UserRoleModel>> GetUserRolesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<List<UserRoleModel>> GetUserRoleModelsByUserAsync(Guid userId, CancellationToken cancellationToken = default);
}
