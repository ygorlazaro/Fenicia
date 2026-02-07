namespace Fenicia.Auth.Domains.Role;

public class RoleService(IRoleRepository roleRepository) : IRoleService
{
    public async Task<string?> GetByUserAndCompanyAsync(Guid userId, Guid companyId, CancellationToken ct)
    {
        var role = await roleRepository.GetByUserAndCompanyAsync(userId, companyId, ct);

        return role;
    }
}