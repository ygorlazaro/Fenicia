using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.DataSource;

/// <summary>
/// Response DTO for dashboard product data source (POS, sales).
/// </summary>
public class DashboardProductDataSourceResponse()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DashboardProductDataSourceResponse"/> class with the specified details.
    /// </summary>
    /// <param name="id">The unique identifier of the product.</param>
    /// <param name="name">The name of the product.</param>
    /// <param name="sku">The SKU of the product (optional).</param>
    /// <param name="barcode">The barcode of the product (optional).</param>
    /// <param name="costPrice">The cost price of the product (optional).</param>
    /// <param name="salesPrice">The sales price of the product.</param>
    /// <param name="quantity">The available quantity of the product.</param>
    /// <param name="unitOfMeasure">The unit of measure (optional).</param>
    /// <param name="categoryId">The unique identifier of the category.</param>
    /// <param name="categoryName">The name of the category.</param>
    /// <param name="isActive">Whether the product is active.</param>
    public DashboardProductDataSourceResponse(
        Guid id,
        string name,
        string? sku,
        string? barcode,
        decimal? costPrice,
        decimal salesPrice,
        double quantity,
        string? unitOfMeasure,
        Guid categoryId,
        string categoryName,
        bool isActive)
        : this()
    {
        Id = id;
        Name = name;
        SKU = sku;
        Barcode = barcode;
        CostPrice = costPrice;
        SalesPrice = salesPrice;
        Quantity = quantity;
        UnitOfMeasure = unitOfMeasure;
        CategoryId = categoryId;
        CategoryName = categoryName;
        IsActive = isActive;
    }

    /// <summary>
    /// Gets or sets the unique identifier of the product.
    /// </summary>
    [Required]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the product.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the SKU of the product.
    /// </summary>
    [MaxLength(50)]
    public string? SKU { get; set; }

    /// <summary>
    /// Gets or sets the barcode of the product.
    /// </summary>
    [MaxLength(50)]
    public string? Barcode { get; set; }

    /// <summary>
    /// Gets or sets the cost price of the product.
    /// </summary>
    public decimal? CostPrice { get; set; }

    /// <summary>
    /// Gets or sets the sales price of the product.
    /// </summary>
    [Required]
    public decimal SalesPrice { get; set; }

    /// <summary>
    /// Gets or sets the available quantity of the product.
    /// </summary>
    [Required]
    public double Quantity { get; set; }

    /// <summary>
    /// Gets or sets the unit of measure.
    /// </summary>
    [MaxLength(20)]
    public string? UnitOfMeasure { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the category.
    /// </summary>
    [Required]
    public Guid CategoryId { get; set; }

    /// <summary>
    /// Gets or sets the name of the category.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string CategoryName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether the product is active.
    /// </summary>
    [Required]
    public bool IsActive { get; set; }
}