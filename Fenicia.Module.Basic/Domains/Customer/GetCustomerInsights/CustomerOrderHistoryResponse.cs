namespace Fenicia.Module.Basic.Domains.Customer.GetCustomerInsights;

public record CustomerOrderHistoryResponse(
    Guid CustomerId,
    string CustomerName,
    int OrderCount,
    decimal TotalSpent,
    int TotalItems,
    DateTime FirstOrderDate,
    DateTime LastOrderDate,
    decimal AverageOrderValue);
