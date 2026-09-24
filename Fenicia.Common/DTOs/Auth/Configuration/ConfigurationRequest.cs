using System.ComponentModel.DataAnnotations;
using Fenicia.Common.Enums.Auth;

namespace Fenicia.Common.DTOs.Auth.Configuration;

/// <summary>
/// Request DTO for creating or updating a user configuration.
/// </summary>
public class ConfigurationRequest()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationRequest"/> class with the specified configuration details.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="configType">The type of configuration.</param>
    /// <param name="value">The configuration value.</param>
    public ConfigurationRequest(Guid userId, EnumConfigType configType, string value)
        : this()
    {
        UserId = userId;
        ConfigType = configType;
        Value = value;
    }

    /// <summary>
    /// Gets or sets the unique identifier of the user.
    /// </summary>
    [Required]
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the type of configuration.
    /// </summary>
    [Required]
    [EnumDataType(typeof(EnumConfigType))]
    public EnumConfigType ConfigType { get; set; }

    /// <summary>
    /// Gets or sets the configuration value.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Value { get; set; } = string.Empty;
}
