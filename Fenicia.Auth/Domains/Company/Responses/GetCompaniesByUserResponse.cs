namespace Fenicia.Auth.Domains.Company.Responses;

public record GetCompaniesByUserResponse(Guid Id, string Name, string Cnpj, string Role);