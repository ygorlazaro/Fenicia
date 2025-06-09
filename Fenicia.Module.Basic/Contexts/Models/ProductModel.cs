using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fenicia.Common.Database;

namespace Fenicia.Module.Basic.Contexts.Models;

[Table("products")]
public class ProductModel : BaseModel
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = null!;

    public decimal CostPrice { get; set; }

    public decimal SellingPrice { get; set; }

    public int Quantity { get; set; }

    [Required]
    public Guid CategoryId { get; set; }

    [ForeignKey("CategoryId")]
    public virtual ProductCategoryModel Category { get; set; } = null!;

    public virtual List<StockMovementModel> StockMovements { get; set; } = null!;
}
