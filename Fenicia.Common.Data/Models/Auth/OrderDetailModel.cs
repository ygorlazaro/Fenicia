using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Fenicia.Common.Data.Models.Auth;

[Table("order_details")]
public class OrderDetailModel : BaseModel
{
    [Required]
    [Column("order_id")]
    public Guid OrderId { get; set; }

    [Required]
    [Column("module_id")]
    public Guid ModuleId { get; set; }

    [Required]
    [Column("price", TypeName = "decimal(18,2)")]
    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }

    [JsonIgnore]
    [ForeignKey(nameof(OrderId))]
    public OrderModel Order { get; set; } = null!;

    [JsonIgnore]
    [ForeignKey(nameof(ModuleId))]
    public ModuleModel Module { get; set; } = null!;

    [JsonIgnore]
    public SubscriptionCreditModel? SubscriptionCredit { get; set; }
}