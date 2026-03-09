namespace Fenicia.Module.Basic.Domains.Customer.GetCustomerInsights;

public record CustomerInsightsResponse
{
    public CustomerSummaryResponse Summary { get; set; } = new();
    public List<CustomerOrderHistoryResponse> TopCustomers { get; set; } = [];
    public List<CustomerRecentOrdersResponse> RecentOrders { get; set; } = [];
    public List<CustomerRiskAlertResponse> AtRiskCustomers { get; set; } = [];
}
