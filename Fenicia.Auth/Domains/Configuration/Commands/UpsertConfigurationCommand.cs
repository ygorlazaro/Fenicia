using System.ComponentModel.DataAnnotations;

using Fenicia.Common.Enums.Auth;

namespace Fenicia.Auth.Domains.Configuration.Commands;

/// <summary>
/// Command to create or update a configuration entry.
/// Uses upsert pattern: creates new if the combination of UserId, CompanyId, and ConfigType doesn't exist,
/// otherwise updates the existing value.
/// </summary>
public record UpsertConfigurationCommand(
    /// <summary>
    /// The configuration ID (optional, used for routing).
    /// </summary>
    Guid? Id,
    /// <summary>
    /// The user ID who owns this configuration.
    /// </summary>
    [Required] Guid UserId,
    /// <summary>
    /// The type of configuration (e.g., Language, Timezone).
    /// </summary>
    [Required] ConfigType ConfigType,
    /// <summary>
    /// The configuration value.
    /// </summary>
    [Required] string Value,
    /// <summary>
    /// The company ID (optional, for company-scoped configurations).
    /// </summary>
    [Required] Guid CompanyId
);
