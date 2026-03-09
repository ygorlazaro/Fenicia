using System.ComponentModel.DataAnnotations.Schema;

using Fenicia.Common.Enums.Auth;

namespace Fenicia.Common.Data.Models.Auth;

[Table("Configuration",  Schema = "auth")]
public class ConfigurationModel: BaseCompanyModel
{
    public ConfigType ConfigType { get; set; }
    
    public string Value { get; set; }
    
    public Guid CompanyId { get; set; }
    
    public Guid UserId { get; set; }
    
    public virtual UserModel User { get; set; }
    
    public virtual CompanyModel Company { get; set; }
}