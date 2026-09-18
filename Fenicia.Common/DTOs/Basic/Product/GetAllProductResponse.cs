using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Product;

public class GetAllProductResponse() : ICrudItem
{
    public GetAllProductResponse(
        Guid id,
        string name,
        string? sku,
        string? barcode,
        string? description,
        decimal? costPrice,
        decimal salesPrice,
        double quantity,
        int? minStockLevel,
        int? maxStockLevel,
        string? imageUrl,
        decimal? weight,
        string? dimensions,
        string? unitOfMeasure,
        Guid categoryId,
        string categoryName,
        Guid? supplierId,
        string? supplierName,
        bool isActive)
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
        CategoryName = categoryName;
        SupplierId = supplierId;
        SupplierName = supplierName;
        IsActive = isActive;
    }

    [Required]
    public Guid Id { get; init; }

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

    [Required]
    [MaxLength(50)]
    public string CategoryName { get; set; } = string.Empty;

    public Guid? SupplierId { get; set; }

    [MaxLength(50)]
    public string? SupplierName { get; set; }

    [Required]
    public bool IsActive { get; set; }
}
