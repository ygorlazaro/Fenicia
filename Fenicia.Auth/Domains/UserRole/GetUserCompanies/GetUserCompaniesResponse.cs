namespace Fenicia.Auth.Domains.UserRole.GetUserCompanies;

public record GetUserCompaniesResponse(Guid Id, string Role, Guid CompanyId, string CompanyName, string Cnpj);