using Fenicia.Common.Enums.Auth;

namespace Fenicia.Auth.Domains.Configuration.Responses;

/// <summary>
///     Response model containing configuration information.
/// </summary>
public record GetConfigurationResponse(
    /// <summary>
    /// The configuration ID.
    /// </summary>
    Guid Id,
    /// <summary>
    /// The user ID who owns this configuration.
    /// </summary>
    Guid UserId,
    /// <summary>
    /// The company ID (now required).
    /// </summary>
    Guid CompanyId,
    /// <summary>
    /// The type of configuration.
    /// </summary>
    ConfigType ConfigType,
    /// <summary>
    /// The configuration value.
    /// </summary>
    string Value);
