using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Product;

/// <summary>
/// Request DTO for creating or updating a product.
/// </summary>
public class ProductRequest()
{
    /// <summary>
    /// Initializes a new instance with the specified details.
    /// </summary>
    /// <param name="id">The product ID (for updates).</param>
    /// <param name="name">The product name.</param>
    /// <param name="sku">The product SKU.</param>
    /// <param name="barcode">The product barcode.</param>
    /// <param name="description">The product description.</param>
    /// <param name="costPrice">The cost price.</param>
    /// <param name="salesPrice">The sales price.</param>
    /// <param name="quantity">The available quantity.</param>
    /// <param name="minStockLevel">The minimum stock level.</param>
    /// <param name="maxStockLevel">The maximum stock level.</param>
    /// <param name="imageUrl">The product image URL.</param>
    /// <param name="weight">The product weight.</param>
    /// <param name="dimensions">The product dimensions.</param>
    /// <param name="unitOfMeasure">The unit of measure.</param>
    /// <param name="categoryId">The category ID.</param>
    /// <param name="supplierId">The supplier ID.</param>
    public ProductRequest(
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

    /// <summary>Gets or sets the product ID.</summary>
    [Required]
    public Guid Id { get; set; }

    /// <summary>Gets or sets the product name.</summary>
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the product SKU.</summary>
    [MaxLength(50)]
    public string? SKU { get; set; }

    /// <summary>Gets or sets the product barcode.</summary>
    [MaxLength(50)]
    public string? Barcode { get; set; }

    /// <summary>Gets or sets the product description.</summary>
    [MaxLength(1000)]
    public string? Description { get; set; }

    /// <summary>Gets or sets the cost price.</summary>
    public decimal? CostPrice { get; set; }

    /// <summary>Gets or sets the sales price.</summary>
    [Required]
    public decimal SalesPrice { get; set; }

    /// <summary>Gets or sets the available quantity.</summary>
    [Required]
    public double Quantity { get; set; }

    /// <summary>Gets or sets the minimum stock level.</summary>
    public int? MinStockLevel { get; set; }

    /// <summary>Gets or sets the maximum stock level.</summary>
    public int? MaxStockLevel { get; set; }

    /// <summary>Gets or sets the product image URL.</summary>
    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    /// <summary>Gets or sets the product weight.</summary>
    public decimal? Weight { get; set; }

    /// <summary>Gets or sets the product dimensions.</summary>
    [MaxLength(50)]
    public string? Dimensions { get; set; }

    /// <summary>Gets or sets the unit of measure.</summary>
    [MaxLength(20)]
    public string? UnitOfMeasure { get; set; }

    /// <summary>Gets or sets the category ID.</summary>
    [Required]
    public Guid CategoryId { get; set; }

    /// <summary>Gets or sets the supplier ID.</summary>
    public Guid? SupplierId { get; set; }
}