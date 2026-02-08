using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Fenicia.Common.Data.Requests.Basic;

namespace Fenicia.Common.Data.Models.Basic;

[Table("products")]
public sealed class ProductModel : BaseModel
{
    public ProductModel(ProductRequest request)
    {
        this.Id = request.Id;
        this.Name = request.Name;
        this.CostPrice = request.CostPrice;
        this.SalesPrice = request.SellingPrice;
        this.Quantity = request.Quantity;
        this.CategoryId = request.CategoryId;
        this.Category = null!;
        this.StockMovements = [];
        this.OrderDetails = [];
    }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; }

    public decimal? CostPrice { get; set; }

    [Required]
    public decimal SalesPrice { get; set; }

    [Required]
    public double Quantity { get; set; }

    [Required]
    public Guid CategoryId { get; set; }

    [ForeignKey(nameof(CategoryId))]
    public ProductCategoryModel Category { get; set; }

    public List<StockMovementModel> StockMovements { get; set; }

    public List<OrderDetailModel> OrderDetails { get; set; }
}