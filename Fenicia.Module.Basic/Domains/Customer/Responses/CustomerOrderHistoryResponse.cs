namespace Fenicia.Module.Basic.Domains.Customer.Responses;

public record CustomerOrderHistoryResponse(
    Guid CustomerId,
    string CustomerName,
    int OrderCount,
    decimal TotalSpent,
    int TotalItems,
    DateTime FirstOrderDate,
    DateTime LastOrderDate,
    decimal AverageOrderValue);
