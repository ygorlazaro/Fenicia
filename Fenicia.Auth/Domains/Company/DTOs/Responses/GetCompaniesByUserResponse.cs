namespace Fenicia.Auth.Domains.Company.DTOs.Responses;

public record GetCompaniesByUserResponse(Guid Id, string Name, string Cnpj, string Role);