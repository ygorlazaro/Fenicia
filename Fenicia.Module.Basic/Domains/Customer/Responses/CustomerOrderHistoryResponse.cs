namespace Fenicia.Module.Basic.Domains.Customer.Responses;

/// <summary>
///     Response model for customer's order history summary.
///     Contains aggregated order statistics for a single customer.
/// </summary>
public record CustomerOrderHistoryResponse(Guid CustomerId, string CustomerName, int OrderCount, decimal TotalSpent, int TotalItems, DateTime FirstOrderDate, DateTime LastOrderDate, decimal AverageOrderValue);