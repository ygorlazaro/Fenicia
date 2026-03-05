using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models;

[Table("products", Schema = "basic")]
public class BasicProductModel : BaseCompanyModel
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = null!;

    public decimal? CostPrice { get; set; }

    [Required]
    public decimal SalesPrice { get; set; }

    [Required]
    public double Quantity { get; set; }

    [Required]
    public Guid CategoryId { get; set; }

    [ForeignKey(nameof(CategoryId))]
    public BasicProductCategoryModel Category { get; set; } = null!;

    public Guid? SupplierId { get; set; }

    [ForeignKey(nameof(SupplierId))]
    public BasicSupplierModel? Supplier { get; set; }

    public List<BasicStockMovementModel> StockMovements { get; set; } = null!;

    public List<BasicOrderDetailModel> OrderDetails { get; set; } = null!;
}
