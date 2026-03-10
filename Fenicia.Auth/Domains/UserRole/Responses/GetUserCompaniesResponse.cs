namespace Fenicia.Auth.Domains.UserRole.Responses;

public record GetUserCompaniesResponse(Guid Id, string Role, Guid CompanyId, string CompanyName, string Cnpj);