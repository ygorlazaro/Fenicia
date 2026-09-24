using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Inventory;

/// <summary>
/// Response DTO containing category breakdown data.
/// </summary>
/// <param name="categoryId">The unique identifier of the category.</param>
/// <param name="categoryName">The name of the category.</param>
/// <param name="totalCostValue">The total cost value.</param>
/// <param name="totalSalesValue">The total sales value.</param>
/// <param name="totalQuantity">The total quantity.</param>
public record CategoryBreakdownResponse(
    [Required] Guid CategoryId,
    [Required] [MaxLength(200)] string CategoryName,
    decimal TotalCostValue,
    decimal TotalSalesValue,
    double TotalQuantity);