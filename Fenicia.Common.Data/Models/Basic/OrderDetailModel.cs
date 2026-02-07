using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Fenicia.Common.Data.Models.Basic;

[Table("order_details")]
public class OrderDetailModel : BaseModel
{
    [Required]
    [Column("order_id")]
    public Guid OrderId
    {
        get; set;
    }

    [Required]
    [Column("product_id")]
    public Guid ProductId
    {
        get; set;
    }

    [Required]
    [Column("price", TypeName = "decimal(18,2)")]
    [Range(0.01, double.MaxValue)]
    public decimal Price
    {
        get; set;
    }

    [JsonIgnore]
    [ForeignKey(nameof(OrderId))]
    public virtual OrderModel Order { get; set; } = null!;

    [JsonIgnore]
    [ForeignKey(nameof(ProductId))]
    public virtual ProductModel Product { get; set; } = null!;

    [Column("quantity")]
    [Range(0.01, double.MaxValue)]
    [Required]
    public double Quantity
    {
        get;
        set;
    }
}