using System.ComponentModel.DataAnnotations;
using Fenicia.Common.Enums.Auth;

namespace Fenicia.Common.DTOs.Auth.Configuration;

/// <summary>
/// Response DTO containing configuration details.
/// </summary>
public class ConfigurationResponse()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationResponse"/> class with the specified configuration details.
    /// </summary>
    /// <param name="id">The unique identifier of the configuration.</param>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="companyId">The unique identifier of the company.</param>
    /// <param name="configType">The type of configuration.</param>
    /// <param name="value">The configuration value.</param>
    public ConfigurationResponse(Guid id, Guid userId, Guid companyId, EnumConfigType configType, string value)
        : this()
    {
        Id = id;
        UserId = userId;
        CompanyId = companyId;
        ConfigType = configType;
        Value = value;
    }

    /// <summary>
    /// Gets or sets the unique identifier of the configuration.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the user.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the company.
    /// </summary>
    public Guid CompanyId { get; set; }

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
