namespace Fenicia.Module.Basic.Domains.Customer.Responses;

/// <summary>
///     Response model containing comprehensive customer analytics and insights.
///     Includes summary statistics, top customers, recent orders, and risk alerts.
/// </summary>
public record CustomerInsightsResponse
{
    /// <summary>Summary statistics for all customers.</summary>
    public CustomerSummaryResponse Summary { get; set; } = new();

    /// <summary>List of top customers by spending.</summary>
    public List<CustomerOrderHistoryResponse> TopCustomers { get; set; } = [];

    /// <summary>List of recent orders across all customers.</summary>
    public List<CustomerRecentOrdersResponse> RecentOrders { get; set; } = [];

    /// <summary>List of customers at risk of churn.</summary>
    public List<CustomerRiskAlertResponse> AtRiskCustomers { get; set; } = [];
}