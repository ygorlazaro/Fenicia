using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Inventory;

/// <summary>
/// Response DTO containing zero movement product data.
/// </summary>
/// <param name="productId">The unique identifier of the product.</param>
/// <param name="productName">The name of the product.</param>
/// <param name="categoryId">The unique identifier of the category.</param>
/// <param name="categoryName">The name of the category.</param>
/// <param name="supplierName">The name of the supplier (optional).</param>
/// <param name="currentStock">The current stock quantity.</param>
/// <param name="stockValue">The stock value.</param>
/// <param name="lastMovementDate">The date of the last movement (optional).</param>
/// <param name="daysWithoutMovement">The number of days without movement.</param>
public record ZeroMovementProductResponse(
    [Required] Guid ProductId,
    [Required][MaxLength(200)] string ProductName,
    [Required] Guid CategoryId,
    [Required][MaxLength(200)] string CategoryName,
    [MaxLength(200)] string? SupplierName,
    double CurrentStock,
    decimal StockValue,
    DateTime? LastMovementDate,
    int DaysWithoutMovement);