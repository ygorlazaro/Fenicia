using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;

namespace Fenicia.Auth.Domains.UserRole.Interfaces;

public interface IUserRoleRepository : IRepository<UserRoleModel>
{
    Task<List<UserRoleModel>> GetCompaniesByUserAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<List<UserRoleModel>> GetUserCompaniesAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<List<UserRoleModel>> GetUserRolesAsync(
        Guid userId,
        int page,
        int perPage,
        CancellationToken cancellationToken = default);

    Task<int> CountUserRolesAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<UserRoleModel?> GetUserRoleAsync(Guid userId, Guid companyId, CancellationToken cancellationToken = default);

    Task<bool> IsAdminAsync(Guid userId, Guid companyId, CancellationToken cancellationToken = default);

    Task<bool> AnyIdAndCompanyAsync(Guid userId, Guid companyId, CancellationToken cancellationToken = default);

    Task<bool> HasRoleAsync(Guid userId, Guid companyId, string role, CancellationToken cancellationToken = default);
}