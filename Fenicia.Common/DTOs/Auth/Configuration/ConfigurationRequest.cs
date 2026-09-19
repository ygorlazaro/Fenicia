using System.ComponentModel.DataAnnotations;
using Fenicia.Common.Enums.Auth;

namespace Fenicia.Common.DTOs.Auth.Configuration;

public class ConfigurationRequest()
{
    public ConfigurationRequest(Guid userId, ConfigType configType, string value)
        : this()
    {
        UserId = userId;
        ConfigType = configType;
        Value = value;
    }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    [EnumDataType(typeof(ConfigType))]
    public ConfigType ConfigType { get; set; }

    [Required]
    [MaxLength(200)]
    public string Value { get; set; } = string.Empty;
}
