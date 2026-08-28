namespace Fenicia.Auth.Domains.Company.DTOs;

public record GetCompaniesByUserResponse(Guid Id, string Name, string Cnpj, string Role);
