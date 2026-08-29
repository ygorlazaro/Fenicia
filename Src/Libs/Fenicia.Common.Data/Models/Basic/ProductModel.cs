using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models.Basic;

[Table("products", Schema = "basic")]
public class ProductModel : BaseCompanyModel
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = null!;

    [MaxLength(50)]
    public string? SKU { get; set; }

    [MaxLength(50)]
    public string? Barcode { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }

    public decimal? CostPrice { get; set; }

    [Required]
    public decimal SalesPrice { get; set; }

    [Required]
    public double Quantity { get; set; }

    [Column("min_stock_level")]
    public int? MinStockLevel { get; set; }

    [Column("max_stock_level")]
    public int? MaxStockLevel { get; set; }

    [Column("image_url")]
    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    [Column("weight")]
    public decimal? Weight { get; set; }

    [Column("dimensions")]
    [MaxLength(50)]
    public string? Dimensions { get; set; }

    [MaxLength(20)]
    public string? UnitOfMeasure { get; set; }

    [Required]
    public Guid CategoryId { get; set; }

    [ForeignKey(nameof(CategoryId))]
    public ProductCategoryModel Category { get; set; } = null!;

    public Guid? SupplierId { get; set; }

    [ForeignKey(nameof(SupplierId))]
    public SupplierModel? Supplier { get; set; }

    [Required]
    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    public List<StockMovementModel> StockMovements { get; set; } = null!;

    public List<OrderDetailModel> OrderDetails { get; set; } = null!;
}