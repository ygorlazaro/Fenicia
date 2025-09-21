namespace Fenicia.Auth.Domains.UserRole;

using Common;

using Fenicia.Common.Database.Responses;

public interface IUserRoleService
{
    Task<ApiResponse<string[]>> GetRolesByUserAsync(Guid userId, CancellationToken cancellationToken);

    Task<ApiResponse<List<CompanyResponse>>> GetUserCompaniesAsync(Guid userId, CancellationToken cancellationToken);

    Task<ApiResponse<bool>> HasRoleAsync(Guid userId, Guid companyId, string role, CancellationToken cancellationToken);
}
