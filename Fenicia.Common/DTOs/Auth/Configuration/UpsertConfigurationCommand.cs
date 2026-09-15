using System.ComponentModel.DataAnnotations;
using Fenicia.Common.Enums.Auth;

namespace Fenicia.Common.DTOs.Auth.Configuration;

public class UpsertConfigurationCommand()
{
    public UpsertConfigurationCommand(Guid? id, Guid userId, ConfigType configType, string value)
        : this()
    {
        Id = id;
        UserId = userId;
        ConfigType = configType;
        Value = value;
    }

    public Guid? Id { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    public ConfigType ConfigType { get; set; }

    [Required]
    [MaxLength(200)]
    public string Value { get; set; } = string.Empty;
}
