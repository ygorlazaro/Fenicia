using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models.Auth;

[Table("order_details", Schema = "auth")]
public class OrderDetailModel : BaseModel
{
    [Required]
    [Column("order_id")]
    public Guid OrderId { get; init; }

    [Required]
    [Column("module_id")]
    public Guid ModuleId { get; init; }

    [Required]
    [Column("price", TypeName = "decimal(18,2)")]
    [Range(0.01, double.MaxValue)]
    public decimal Price { get; init; }

    [Column("discount_amount", TypeName = "decimal(18,2)")]
    [Range(0, double.MaxValue)]
    public decimal DiscountAmount { get; init; }

    [Column("subtotal", TypeName = "decimal(18,2)")]
    [Range(0, double.MaxValue)]
    public decimal Subtotal { get; init; }

    [ForeignKey(nameof(OrderId))]
    public OrderModel Order { get; init; } = default!;

    [ForeignKey(nameof(ModuleId))]
    public ModuleModel Module { get; init; } = default!;

    public SubscriptionCreditModel? SubscriptionCredit { get; init; }
}