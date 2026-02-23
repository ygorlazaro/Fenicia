namespace Fenicia.Auth.Domains.UserRole.HasRole;

public record HasRoleQuery(Guid UserId, Guid CompanyId, string Role);