namespace Fenicia.Auth.Domains.UserRole.GetUserCompanies;

public record GetUserCompaniesResponse(Guid Id, string Role, CompanyResponse Company);