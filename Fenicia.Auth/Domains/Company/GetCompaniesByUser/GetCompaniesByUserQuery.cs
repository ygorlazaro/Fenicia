namespace Fenicia.Auth.Domains.Company.GetCompaniesByUser;

public sealed record GetCompaniesByUserQuery(Guid UserId, int Page, int PerPage);
