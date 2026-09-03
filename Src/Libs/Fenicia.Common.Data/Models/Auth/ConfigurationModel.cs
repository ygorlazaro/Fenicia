using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fenicia.Common.Enums.Auth;

namespace Fenicia.Common.Data.Models.Auth;

[Table("Configuration", Schema = "auth")]
public sealed class ConfigurationModel : BaseCompanyModel
{
    public ConfigType ConfigType { get; init; }

    [MaxLength(200)]
    public string Value { get; set; } = string.Empty;

    public Guid UserId { get; init; }

    public UserModel User { get; init; } = default!;

    public CompanyModel Company { get; init; } = default!;
}