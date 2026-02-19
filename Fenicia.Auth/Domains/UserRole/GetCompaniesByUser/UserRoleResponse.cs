namespace Fenicia.Auth.Domains.UserRole.GetCompaniesByUser;

public record UserRoleResponse(Guid Id, string Role, CompanyResponse Company);