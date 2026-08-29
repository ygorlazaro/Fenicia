using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models.Auth;

[Table("order_details", Schema = "auth")]
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

    [Column("discount_amount", TypeName = "decimal(18,2)")]
    [Range(0, double.MaxValue)]
    public decimal DiscountAmount { get; set; }

    [Column("subtotal", TypeName = "decimal(18,2)")]
    [Range(0, double.MaxValue)]
    public decimal Subtotal { get; set; }

    [ForeignKey(nameof(OrderId))]
    public OrderModel Order { get; set; } = null!;

    [ForeignKey(nameof(ModuleId))]
    public ModuleModel Module { get; set; } = null!;

    public SubscriptionCreditModel? SubscriptionCredit { get; set; }
}