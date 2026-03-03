using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Fenicia.Common.Data.Models;

[Table("order_details", Schema = "basic")]
public class BasicOrderDetailModel : BaseCompanyModel
{
    [Required]
    [Column("order_id")]
    public Guid OrderId { get; set; }

    [Required]
    [Column("product_id")]
    public Guid ProductId { get; set; }

    [Required]
    [Column("price", TypeName = "decimal(18,2)")]
    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }

    [JsonIgnore]
    [ForeignKey(nameof(OrderId))]
    public BasicOrderModel OrderModel { get; set; } = null!;

    [JsonIgnore]
    [ForeignKey(nameof(ProductId))]
    public BasicProductModel ProductModel { get; set; } = null!;

    [Column("quantity")]
    [Range(0.01, double.MaxValue)]
    [Required]
    public double Quantity { get; set; }
}
