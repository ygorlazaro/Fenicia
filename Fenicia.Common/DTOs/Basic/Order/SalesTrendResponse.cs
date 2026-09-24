using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Order;

/// <summary>
/// Response DTO containing sales trend data.
/// </summary>
/// <param name="period">The period label.</param>
/// <param name="date">The date of the period.</param>
/// <param name="orderCount">The number of orders.</param>
/// <param name="totalValue">The total value of orders.</param>
/// <param name="totalItems">The total number of items.</param>
public record SalesTrendResponse(
    [Required] [MaxLength(200)] string Period,
    [Required] DateTime Date,
    int OrderCount,
    decimal TotalValue,
    int TotalItems);