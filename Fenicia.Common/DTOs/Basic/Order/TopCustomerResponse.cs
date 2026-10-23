using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Order;

/// <summary>
/// Response DTO containing top customer data.
/// </summary>
/// <param name="customerId">The unique identifier of the customer.</param>
/// <param name="customerName">The name of the customer.</param>
/// <param name="orderCount">The number of orders.</param>
/// <param name="totalSpent">The total amount spent.</param>
/// <param name="totalItems">The total number of items.</param>
public record TopCustomerResponse(
    [Required] Guid CustomerId,
    [Required][MaxLength(200)] string CustomerName,
    int OrderCount,
    decimal TotalSpent,
    int TotalItems);