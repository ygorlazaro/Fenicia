namespace Fenicia.Auth.Domains.Company.Responses;

/// <summary>
/// Response model containing company information for a user.
/// </summary>
public record GetCompaniesByUserResponse(Guid Id, string Name, string Cnpj, string Role);