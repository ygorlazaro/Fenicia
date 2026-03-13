namespace Fenicia.Auth.Domains.Company.Queries;

/// <summary>
/// Query to check if a company exists in the system based on its CNPJ.
/// </summary>
/// <remarks>
/// Used to validate CNPJ uniqueness during company registration or updates.
/// </remarks>
public record CheckCompanyExistsQuery(string Cnpj, bool OnlyActive);