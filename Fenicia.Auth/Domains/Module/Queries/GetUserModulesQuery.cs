namespace Fenicia.Auth.Domains.Module.Queries;

/// <summary>
///     Query to retrieve modules available to a user within a specific company.
/// </summary>
/// <remarks>
///     Used by GetUserModuleHandler to determine which modules a user can access
///     based on their subscriptions and subscription credits.
/// </remarks>
public record GetUserModulesQuery(Guid CompanyId, Guid UserId);