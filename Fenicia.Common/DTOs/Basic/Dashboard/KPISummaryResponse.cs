namespace Fenicia.Common.DTOs.Basic.Dashboard;

/// <summary>
/// Response DTO containing KPI summary metrics.
/// </summary>
public class KpiSummaryResponse
{
    /// <summary>
    /// Gets or sets the total revenue.
    /// </summary>
    public decimal TotalRevenue { get; set; }

    /// <summary>
    /// Gets or sets the total cost.
    /// </summary>
    public decimal TotalCost { get; set; }

    /// <summary>
    /// Gets or sets the gross profit.
    /// </summary>
    public decimal GrossProfit { get; set; }

    /// <summary>
    /// Gets or sets the profit margin percentage.
    /// </summary>
    public decimal ProfitMargin { get; set; }

    /// <summary>
    /// Gets or sets the total number of orders.
    /// </summary>
    public int TotalOrders { get; set; }

    /// <summary>
    /// Gets or sets the total number of products.
    /// </summary>
    public int TotalProducts { get; set; }

    /// <summary>
    /// Gets or sets the average order value.
    /// </summary>
    public decimal AverageOrderValue { get; set; }

    /// <summary>
    /// Gets or sets the total stock value.
    /// </summary>
    public decimal TotalStockValue { get; set; }
}