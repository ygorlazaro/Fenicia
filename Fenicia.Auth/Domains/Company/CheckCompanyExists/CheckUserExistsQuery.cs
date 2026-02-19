namespace Fenicia.Auth.Domains.Company.CheckCompanyExists;

public record CheckUserExistsQuery(string Cnpj, bool OnlyActive);