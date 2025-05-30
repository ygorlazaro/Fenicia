using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Fenicia.Common.Database;

namespace Fenicia.Auth.Contexts.Models;

[Table("order_details")]
public class OrderDetailModel : BaseModel
{
    [Required]
    public Guid OrderId { get; set; }
    
    [Required]
    public Guid ModuleId { get; set; }
    
    [Required]
    public decimal Amount { get; set; }
    
    [JsonIgnore]
    [ForeignKey("OrderId")]
    public virtual OrderModel Order { get; set; } = null!;
    
    [JsonIgnore]
    [ForeignKey("ModuleId")]
    public virtual ModuleModel Module { get; set; } = null!;
    
    [JsonIgnore]
    public virtual SubscriptionCreditModel? SubscriptionCredit { get; set; }
}