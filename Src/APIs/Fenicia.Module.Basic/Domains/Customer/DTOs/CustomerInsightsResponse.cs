namespace Fenicia.Module.Basic.Domains.Customer.DTOs;

public record CustomerInsightsResponse
{
    public CustomerSummaryResponse Summary { get; init; } = new();

    public List<CustomerOrderHistoryResponse> TopCustomers { get; init; } = [];

    public List<CustomerRecentOrdersResponse> RecentOrders { get; init; } = [];

    public List<CustomerRiskAlertResponse> AtRiskCustomers { get; init; } = [];
}