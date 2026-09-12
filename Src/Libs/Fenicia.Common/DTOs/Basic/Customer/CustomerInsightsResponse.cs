namespace Fenicia.Common.DTOs.Basic.Customer;

public record CustomerInsightsResponse
{
    public CustomerSummaryResponse Summary { get; init; } = new();

    public List<CustomerOrderHistoryResponse> TopCustomers { get; init; } = [];

    public List<CustomerRecentOrdersResponse> RecentOrders { get; init; } = [];

    public List<CustomerRiskAlertResponse> AtRiskCustomers { get; init; } = [];
}