namespace Fenicia.Auth.Domains.Company.CheckCompanyExists;

public record CheckCompanyExistsQuery(string Cnpj, bool OnlyActive);