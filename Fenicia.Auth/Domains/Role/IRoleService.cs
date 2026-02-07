namespace Fenicia.Auth.Domains.Role;

public interface IRoleService
{
    Task<string?> GetByUserAndCompanyAsync(Guid userId, Guid companyId, CancellationToken ct);
}