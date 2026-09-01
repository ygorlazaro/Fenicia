using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Fenicia.Common.Enums.Auth;

namespace Fenicia.Common.Data.Models.Auth;

[Table("Configuration", Schema = "auth")]
public class ConfigurationModel : BaseCompanyModel
{
    public ConfigType ConfigType { get; set; }

    [MaxLength(200)]
    public string Value { get; set; } = null!;

    public Guid UserId { get; set; }

    public virtual UserModel User { get; set; } = null!;

    public virtual CompanyModel Company { get; set; } = null!;
}
