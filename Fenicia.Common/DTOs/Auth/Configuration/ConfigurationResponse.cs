using System.ComponentModel.DataAnnotations;
using Fenicia.Common.Enums.Auth;

namespace Fenicia.Common.DTOs.Auth.Configuration;

public class ConfigurationResponse()
{
    public ConfigurationResponse(Guid id, Guid userId, Guid companyId, EnumConfigType configType, string value)
        : this()
    {
        Id = id;
        UserId = userId;
        CompanyId = companyId;
        ConfigType = configType;
        Value = value;
    }

    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid CompanyId { get; set; }

    [Required]
    [EnumDataType(typeof(EnumConfigType))]
    public EnumConfigType ConfigType { get; set; }

    [Required]
    [MaxLength(200)]
    public string Value { get; set; } = string.Empty;
}
