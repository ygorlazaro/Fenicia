namespace Fenicia.Auth.Domains.Configuration.Queries;

/// <summary>
/// Query to retrieve user configurations.
/// </summary>
public record GetConfigurationQuery(
    /// <summary>
    /// The user ID to retrieve configurations for.
    /// </summary>
    Guid UserId,
    /// <summary>
    /// Optional company ID to filter configurations.
    /// </summary>
    Guid? CompanyId = null
);
