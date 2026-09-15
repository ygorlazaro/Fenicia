using System.ComponentModel.DataAnnotations;
using Fenicia.Common.Enums.Auth;

namespace Fenicia.Common.DTOs.Auth.Configuration;

public class GetConfigurationResponse()
{
    public GetConfigurationResponse(Guid id, Guid userId, Guid companyId, ConfigType configType, string value)
        : this()
    {
        Id = id;
        UserId = userId;
        CompanyId = companyId;
        ConfigType = configType;
        Value = value;
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid CompanyId { get; set; }

    [Required]
    public ConfigType ConfigType { get; set; }

    [Required]
    [MaxLength(200)]
    public string Value { get; set; } = string.Empty;
}
