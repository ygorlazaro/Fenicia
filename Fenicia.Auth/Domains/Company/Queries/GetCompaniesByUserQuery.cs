namespace Fenicia.Auth.Domains.Company.Queries;

public sealed record GetCompaniesByUserQuery(Guid UserId, int Page, int PerPage);