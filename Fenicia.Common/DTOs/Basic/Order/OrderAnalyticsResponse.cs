namespace Fenicia.Common.DTOs.Basic.Order;

/// <summary>
/// Response DTO containing order analytics data.
/// </summary>
public class OrderAnalyticsResponse
{
    /// <summary>
    /// Gets or sets the orders grouped by status.
    /// </summary>
    public List<OrderStatusCountResponse> OrdersByStatus { get; set; } = [];

    /// <summary>
    /// Gets or sets the sales trend data.
    /// </summary>
    public List<SalesTrendResponse> SalesTrend { get; set; } = [];

    /// <summary>
    /// Gets or sets the top customers.
    /// </summary>
    public List<TopCustomerResponse> TopCustomers { get; set; } = [];

    /// <summary>
    /// Gets or sets the average order value metrics.
    /// </summary>
    public AverageOrderValueResponse AverageOrderValue { get; set; } = new(0, 0, 0, 0, 0);

    /// <summary>
    /// Gets or sets the cancelled orders.
    /// </summary>
    public List<CancelledOrderResponse> CancelledOrders { get; set; } = [];
}