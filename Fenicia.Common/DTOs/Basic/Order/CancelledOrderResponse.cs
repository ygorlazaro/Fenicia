using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Order;

/// <summary>
/// Response DTO containing cancelled order data.
/// </summary>
/// <param name="orderId">The unique identifier of the order.</param>
/// <param name="customerName">The name of the customer.</param>
/// <param name="totalAmount">The total amount of the order.</param>
/// <param name="saleDate">The date of the sale.</param>
/// <param name="totalItems">The total number of items.</param>
/// <param name="cancelledReason">The reason for cancellation (optional).</param>
public record CancelledOrderResponse(
    [Required] Guid OrderId,
    [Required] [MaxLength(200)] string CustomerName,
    [Range(0, double.MaxValue)] decimal TotalAmount,
    [Required] DateTime SaleDate,
    int TotalItems,
    string? CancelledReason);