using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Inventory;

/// <summary>
/// Response DTO containing inventory item details.
/// </summary>
/// <param name="id">The unique identifier of the product.</param>
/// <param name="name">The name of the product.</param>
/// <param name="quantity">The available quantity.</param>
/// <param name="costPrice">The cost price (optional).</param>
/// <param name="salesPrice">The sales price.</param>
/// <param name="categoryId">The unique identifier of the category.</param>
/// <param name="categoryName">The name of the category.</param>
public record InventoryDetailResponse(
    [Required] Guid Id,
    [Required][MaxLength(200)] string Name,
    double Quantity,
    decimal? CostPrice,
    [Required] decimal SalesPrice,
    [Required] Guid CategoryId,
    [Required][MaxLength(200)] string CategoryName);