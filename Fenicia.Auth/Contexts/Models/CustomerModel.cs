using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Fenicia.Common.Database;

namespace Fenicia.Auth.Contexts.Models;

[Table("customers")]
public class CustomerModel : BaseModel
{
    [Required]
    public Guid UserId { get; set; }
    
    [Required]
    public Guid CompanyId { get; set; }
    
    [JsonIgnore]
    [ForeignKey("UserId")]
    public virtual UserModel User { get; set; } = null!;
    
    [JsonIgnore]
    [ForeignKey("CompanyId")]
    public virtual CompanyModel Company { get; set; } = null!;
    
    [JsonIgnore]
    public virtual List<OrderModel> Orders { get; set; } = null!;
}