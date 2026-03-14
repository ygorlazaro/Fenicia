namespace Fenicia.Module.Basic.Domains.Customer.Responses;

/// <summary>
///     Response model for recent orders in customer insights.
///     Contains order details for recent transactions.
/// </summary>
public record CustomerRecentOrdersResponse(Guid OrderId, Guid CustomerId, string CustomerName, decimal TotalAmount, DateTime SaleDate, string Status, int TotalItems);