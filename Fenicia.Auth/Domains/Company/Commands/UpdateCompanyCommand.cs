namespace Fenicia.Auth.Domains.Company.Commands;

public sealed record UpdateCompanyCommand(Guid CompanyId, Guid UserId, string Name);
