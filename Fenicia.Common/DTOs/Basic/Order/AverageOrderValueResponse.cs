namespace Fenicia.Common.DTOs.Basic.Order;

/// <summary>
/// Response DTO containing average order value metrics.
/// </summary>
public record AverageOrderValueResponse(
    decimal AverageValue,
    int TotalOrders,
    decimal MedianValue,
    decimal MinValue,
    decimal MaxValue);