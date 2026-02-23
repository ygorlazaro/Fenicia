namespace Fenicia.Auth.Domains.Company.GetCompaniesByUser;

public record GetCompaniesByUserResponse(Guid Id, string Name, string Cnpj, string Role);