using Fenicia.Common;

using Fenicia.Common.Database.Responses;

namespace Fenicia.Auth.Domains.UserRole;

public interface IUserRoleService
{
    Task<ApiResponse<string[]>> GetRolesByUserAsync(Guid userId, CancellationToken cancellationToken);

    Task<ApiResponse<List<CompanyResponse>>> GetUserCompaniesAsync(Guid userId, CancellationToken cancellationToken);

    Task<ApiResponse<bool>> HasRoleAsync(Guid userId, Guid companyId, string role, CancellationToken cancellationToken);
}
