namespace Fenicia.Auth.Domains.Company.UpdateCompany;

public sealed record UpdateCompanyCommand(Guid CompanyId, Guid UserId, string Name, string TimeZone);
