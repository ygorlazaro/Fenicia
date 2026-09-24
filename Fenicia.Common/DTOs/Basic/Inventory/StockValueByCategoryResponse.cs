using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Inventory;

/// <summary>
/// Response DTO containing stock value by category.
/// </summary>
/// <param name="categoryId">The unique identifier of the category.</param>
/// <param name="categoryName">The name of the category.</param>
/// <param name="productCount">The number of products in the category.</param>
/// <param name="totalStockValue">The total stock value.</param>
/// <param name="percentage">The percentage of total stock value.</param>
public class StockValueByCategoryResponse(
    [Required] Guid CategoryId,
    [Required] [MaxLength(200)] string CategoryName,
    int ProductCount,
    decimal TotalStockValue,
    double Percentage)
{
    /// <summary>
    /// Gets or sets the unique identifier of the category.
    /// </summary>
    [Required]
    public Guid CategoryId { get; set; } = CategoryId;

    /// <summary>
    /// Gets or sets the name of the category.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string CategoryName { get; set; } = CategoryName;

    /// <summary>
    /// Gets or sets the number of products in the category.
    /// </summary>
    public int ProductCount { get; set; } = ProductCount;

    /// <summary>
    /// Gets or sets the total stock value.
    /// </summary>
    public decimal TotalStockValue { get; set; } = TotalStockValue;

    /// <summary>
    /// Gets or sets the percentage of total stock value.
    /// </summary>
    public double Percentage { get; set; } = Percentage;
}