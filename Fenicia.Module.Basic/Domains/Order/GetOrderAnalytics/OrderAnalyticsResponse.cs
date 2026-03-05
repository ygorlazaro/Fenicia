namespace Fenicia.Module.Basic.Domains.Order.GetOrderAnalytics;

public record OrderAnalyticsResponse
{
    public List<OrderStatusCountResponse> OrdersByStatus { get; set; } = [];
    public List<SalesTrendResponse> SalesTrend { get; set; } = [];
    public List<TopCustomerResponse> TopCustomers { get; set; } = [];
    public AverageOrderValueResponse AverageOrderValue { get; set; } = new();
    public List<CancelledOrderResponse> CancelledOrders { get; set; } = [];
}

public record OrderStatusCountResponse(
    string Status,
    int Count,
    decimal TotalValue);

public record SalesTrendResponse(
    string Period,
    DateTime Date,
    int OrderCount,
    decimal TotalValue,
    int TotalItems);

public record TopCustomerResponse(
    Guid CustomerId,
    string CustomerName,
    int OrderCount,
    decimal TotalSpent,
    int TotalItems);

public record AverageOrderValueResponse
{
    public decimal AverageValue { get; set; }
    public int TotalOrders { get; set; }
    public decimal MedianValue { get; set; }
    public decimal MinValue { get; set; }
    public decimal MaxValue { get; set; }
}

public record CancelledOrderResponse(
    Guid OrderId,
    string CustomerName,
    decimal TotalAmount,
    DateTime SaleDate,
    int TotalItems,
    string? CancelledReason);
