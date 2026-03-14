namespace Fenicia.Auth.Domains.Company.Queries;

/// <summary>
///     Query to retrieve companies associated with a specific user.
/// </summary>
/// <remarks>
///     Returns companies where the user has an active role, supporting pagination.
/// </remarks>
public sealed record GetCompaniesByUserQuery(Guid UserId, int Page, int PerPage);