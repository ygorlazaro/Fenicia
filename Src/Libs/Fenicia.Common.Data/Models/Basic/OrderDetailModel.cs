using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models.Basic;

[Table("order_details", Schema = "basic")]
public class OrderDetailModel : BaseCompanyModel
{
    [Required]
    [Column("order_id")]
    public Guid OrderId { get; init; }

    [Required]
    [Column("product_id")]
    public Guid ProductId { get; init; }

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

    [ForeignKey(nameof(ProductId))]
    public ProductModel Product { get; init; } = default!;

    [Column("quantity")]
    [Range(0.01, double.MaxValue)]
    [Required]
    public double Quantity { get; init; }
}