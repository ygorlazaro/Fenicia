using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Customer;

/// <summary>
/// Response DTO containing customer insights and analytics.
/// </summary>
public class CustomerInsightsResponse
{
    /// <summary>
    /// Gets or sets the summary of customer metrics.
    /// </summary>
    public CustomerSummaryResponse Summary { get; set; } = new();

    /// <summary>
    /// Gets or sets the top customers by order history.
    /// </summary>
    public List<CustomerOrderHistoryResponse> TopCustomers { get; set; } = [];

    /// <summary>
    /// Gets or sets the recent orders.
    /// </summary>
    public List<CustomerRecentOrdersResponse> RecentOrders { get; set; } = [];

    /// <summary>
    /// Gets or sets the customers at risk of churning.
    /// </summary>
    public List<CustomerRiskAlertResponse> AtRiskCustomers { get; set; } = [];
}