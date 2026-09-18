namespace Fenicia.Common.DTOs.Basic.Customer;

public class CustomerInsightsResponse
{
    public CustomerSummaryResponse Summary { get; set; } = new();

    public List<CustomerOrderHistoryResponse> TopCustomers { get; set; } = [];

    public List<CustomerRecentOrdersResponse> RecentOrders { get; set; } = [];

    public List<CustomerRiskAlertResponse> AtRiskCustomers { get; set; } = [];
}
