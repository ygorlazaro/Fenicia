namespace Fenicia.Module.Basic.Domains.Dashboard.Responses;

/// <summary>
///     Response model containing key performance indicators for the dashboard.
///     Provides aggregate business metrics.
/// </summary>
public record KpiSummaryResponse
{
    /// <summary>Total revenue from all orders.</summary>
    public decimal TotalRevenue { get; set; }

    /// <summary>Total estimated cost from all orders.</summary>
    public decimal TotalCost { get; set; }

    /// <summary>Gross profit (revenue - cost).</summary>
    public decimal GrossProfit { get; set; }

    /// <summary>Profit margin percentage.</summary>
    public decimal ProfitMargin { get; set; }

    /// <summary>Total number of orders.</summary>
    public int TotalOrders { get; set; }

    /// <summary>Total number of products.</summary>
    public int TotalProducts { get; set; }

    /// <summary>Average order value.</summary>
    public decimal AverageOrderValue { get; set; }

    /// <summary>Total value of current stock.</summary>
    public decimal TotalStockValue { get; set; }
}