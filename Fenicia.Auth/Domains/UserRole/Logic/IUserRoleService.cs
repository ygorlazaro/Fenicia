namespace Fenicia.Auth.Domains.UserRole.Logic;

using Common;

public interface IUserRoleService
{
    Task<ApiResponse<string[]>> GetRolesByUserAsync(Guid userId, CancellationToken cancellationToken);

    Task<ApiResponse<bool>> HasRoleAsync(Guid userId, Guid companyId, string role, CancellationToken cancellationToken);
}
