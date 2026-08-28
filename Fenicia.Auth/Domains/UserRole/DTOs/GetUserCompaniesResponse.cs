namespace Fenicia.Auth.Domains.UserRole.DTOs;

public record GetUserCompaniesResponse(Guid Id, string Role, Guid CompanyId, string CompanyName, string Cnpj);
