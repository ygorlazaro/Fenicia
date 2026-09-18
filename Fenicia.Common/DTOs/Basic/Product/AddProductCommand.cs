using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Product;

public class AddProductCommand()
{
    public AddProductCommand(
        Guid id,
        string name,
        string? sku = null,
        string? barcode = null,
        string? description = null,
        decimal? costPrice = null,
        decimal salesPrice = 0,
        double quantity = 0,
        int? minStockLevel = null,
        int? maxStockLevel = null,
        string? imageUrl = null,
        decimal? weight = null,
        string? dimensions = null,
        string? unitOfMeasure = null,
        Guid categoryId = default,
        Guid? supplierId = null)
        : this()
    {
        Id = id;
        Name = name;
        SKU = sku;
        Barcode = barcode;
        Description = description;
        CostPrice = costPrice;
        SalesPrice = salesPrice;
        Quantity = quantity;
        MinStockLevel = minStockLevel;
        MaxStockLevel = maxStockLevel;
        ImageUrl = imageUrl;
        Weight = weight;
        Dimensions = dimensions;
        UnitOfMeasure = unitOfMeasure;
        CategoryId = categoryId;
        SupplierId = supplierId;
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

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

    public int? MinStockLevel { get; set; }

    public int? MaxStockLevel { get; set; }

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    public decimal? Weight { get; set; }

    [MaxLength(50)]
    public string? Dimensions { get; set; }

    [MaxLength(20)]
    public string? UnitOfMeasure { get; set; }

    [Required]
    public Guid CategoryId { get; set; }

    public Guid? SupplierId { get; set; }
}
