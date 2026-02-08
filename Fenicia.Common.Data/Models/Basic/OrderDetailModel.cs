using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Fenicia.Common.Data.Requests.Basic;

namespace Fenicia.Common.Data.Models.Basic;

[Table("order_details")]
public sealed class OrderDetailModel : BaseModel
{
    public OrderDetailModel(OrderDetailRequest request)
    {
        this.OrderId = request.OrderId;
        this.ProductId = request.ProductId;
        this.Price = request.Price;
        this.Quantity = request.Quantity;
        this.Id = request.Id;
    }

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
    public OrderModel Order { get; set; } = null!;

    [JsonIgnore]
    [ForeignKey(nameof(ProductId))]
    public ProductModel Product { get; set; } = null!;

    [Column("quantity")]
    [Range(0.01, double.MaxValue)]
    [Required]
    public double Quantity { get; set; }
}
