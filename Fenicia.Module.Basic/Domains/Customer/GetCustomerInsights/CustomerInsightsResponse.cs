namespace Fenicia.Module.Basic.Domains.Customer.GetCustomerInsights;

public record CustomerInsightsResponse
{
    public CustomerSummaryResponse Summary { get; set; } = new();
    public List<CustomerOrderHistoryResponse> TopCustomers { get; set; } = [];
    public List<CustomerRecentOrdersResponse> RecentOrders { get; set; } = [];
    public List<CustomerRiskAlertResponse> AtRiskCustomers { get; set; } = [];
}

public record CustomerSummaryResponse
{
    public int TotalCustomers { get; set; }
    public int TotalOrders { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal AverageOrderValue { get; set; }
    public decimal AverageCustomerLifetimeValue { get; set; }
}

public record CustomerOrderHistoryResponse(
    Guid CustomerId,
    string CustomerName,
    int OrderCount,
    decimal TotalSpent,
    int TotalItems,
    DateTime FirstOrderDate,
    DateTime LastOrderDate,
    decimal AverageOrderValue);

public record CustomerRecentOrdersResponse(
    Guid OrderId,
    Guid CustomerId,
    string CustomerName,
    decimal TotalAmount,
    DateTime SaleDate,
    string Status,
    int TotalItems);

public record CustomerRiskAlertResponse(
    Guid CustomerId,
    string CustomerName,
    int PreviousOrderCount,
    DateTime LastOrderDate,
    int DaysSinceLastOrder,
    decimal PreviousTotalSpent,
    string RiskLevel);
