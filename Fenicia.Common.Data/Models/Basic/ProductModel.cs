using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models.Basic;

[Table("products")]
public class ProductModel : BaseModel
{
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