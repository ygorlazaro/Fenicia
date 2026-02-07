using Fenicia.Common.Data.Mappers.Auth;
using Fenicia.Common.Data.Responses.Auth;

namespace Fenicia.Auth.Domains.UserRole;

public class UserRoleService(IUserRoleRepository userRoleRepository) : IUserRoleService
{
    public async Task<string[]> GetRolesByUserAsync(Guid userId, CancellationToken ct)
    {
        return await userRoleRepository.GetRolesByUserAsync(userId, ct);
    }

    public async Task<List<CompanyResponse>> GetUserCompaniesAsync(Guid userId, CancellationToken ct)
    {
        var userRoles = await userRoleRepository.GetUserCompaniesAsync(userId, ct);

        return CompanyMapper.Map(userRoles);
    }

    public async Task<bool> HasRoleAsync(Guid userId, Guid companyId, string role, CancellationToken ct)
    {
        return await userRoleRepository.HasRoleAsync(userId, companyId, role, ct);
    }
}
