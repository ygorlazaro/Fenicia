using Fenicia.Auth.Domains.UserRole.DTOs;

namespace Fenicia.Auth.Domains.UserRole;

public class UserRoleService(UserRoleRepository userRoleRepository)
{
    public async Task<List<UserRoleResponse>> GetCompaniesByUserAsync(Guid userId, CancellationToken ct)
    {
        return await userRoleRepository.GetCompaniesByUserAsync(userId, ct);
    }

    public async Task<List<GetUserCompaniesResponse>> GetUserCompaniesAsync(Guid userId, CancellationToken ct)
    {
        return await userRoleRepository.GetUserCompaniesAsync(userId, ct);
    }

    public async Task<bool> AnyIdAndCompanyAsync(Guid userId, Guid companyId, CancellationToken ct)
    {
        return await userRoleRepository.AnyIdAndCompanyAsync(userId, companyId, ct);
    }

    public async Task<bool> HasRoleAsync(Guid userId, Guid companyId, string role, CancellationToken ct)
    {
        return await userRoleRepository.HasRoleAsync(userId, companyId, role, ct);
    }
}
