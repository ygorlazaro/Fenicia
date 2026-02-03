using Fenicia.Common;

using Fenicia.Common.Database.Models.Auth;
using Fenicia.Common.Database.Responses;

namespace Fenicia.Auth.Domains.UserRole;

public class UserRoleService(IUserRoleRepository userRoleRepository) : IUserRoleService
{
    public async Task<ApiResponse<string[]>> GetRolesByUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        var roles = await userRoleRepository.GetRolesByUserAsync(userId, cancellationToken);

        return new ApiResponse<string[]>(roles);
    }

    public async Task<ApiResponse<List<CompanyResponse>>> GetUserCompaniesAsync(Guid userId, CancellationToken cancellationToken)
    {
        var userRoles = await userRoleRepository.GetUserCompaniesAsync(userId, cancellationToken);

        return new ApiResponse<List<CompanyResponse>>(CompanyModel.Convert(userRoles));
    }

    public async Task<ApiResponse<bool>> HasRoleAsync(Guid userId, Guid companyId, string role, CancellationToken cancellationToken)
    {
        var response = await userRoleRepository.HasRoleAsync(userId, companyId, role, cancellationToken);

        return new ApiResponse<bool>(response);
    }
}
