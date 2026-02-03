using Fenicia.Common.Database.Models.Auth;
using Fenicia.Common.Database.Responses;

namespace Fenicia.Auth.Domains.UserRole;

public class UserRoleService(IUserRoleRepository userRoleRepository) : IUserRoleService
{
    public async Task<string[]> GetRolesByUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await userRoleRepository.GetRolesByUserAsync(userId, cancellationToken);
    }

    public async Task<List<CompanyResponse>> GetUserCompaniesAsync(Guid userId, CancellationToken cancellationToken)
    {
        var userRoles = await userRoleRepository.GetUserCompaniesAsync(userId, cancellationToken);

        return CompanyModel.Convert(userRoles);
    }

    public async Task<bool> HasRoleAsync(Guid userId, Guid companyId, string role, CancellationToken cancellationToken)
    {
        return await userRoleRepository.HasRoleAsync(userId, companyId, role, cancellationToken);
    }
}
