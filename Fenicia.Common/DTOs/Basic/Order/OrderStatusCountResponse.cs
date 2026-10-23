using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Order;

/// <summary>
/// Response DTO containing order status count data.
/// </summary>
/// <param name="status">The order status.</param>
/// <param name="count">The count of orders with this status.</param>
/// <param name="totalValue">The total value of orders with this status.</param>
public record OrderStatusCountResponse(
    [Required][MaxLength(200)] string Status,
    int Count,
    [Range(0, double.MaxValue)] decimal TotalValue);