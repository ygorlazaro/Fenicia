namespace Fenicia.Common.DTOs.Basic.Dashboard;

/// <summary>
/// Response DTO containing the complete financial dashboard data.
/// </summary>
public class FinancialDashboardResponse
{
    /// <summary>
    /// Gets or sets the KPI summary.
    /// </summary>
    public KpiSummaryResponse Kpi { get; set; } = new();

    /// <summary>
    /// Gets or sets the revenue vs cost comparison data.
    /// </summary>
    public List<RevenueVsCostResponse> RevenueVsCost { get; set; } = [];

    /// <summary>
    /// Gets or sets the profit margin trend data.
    /// </summary>
    public List<ProfitMarginTrendResponse> ProfitMarginTrend { get; set; } = [];

    /// <summary>
    /// Gets or sets the accounts receivable data.
    /// </summary>
    public AccountsReceivableResponse AccountsReceivable { get; set; } = new();

    /// <summary>
    /// Gets or sets the daily sales summary.
    /// </summary>
    public DailySalesSummaryResponse DailySales { get; set; } = new();

    /// <summary>
    /// Gets or sets the top categories by revenue.
    /// </summary>
    public List<CategoryBreakdownResponse> TopCategoriesByRevenue { get; set; } = [];

    /// <summary>
    /// Gets or sets the top categories by quantity.
    /// </summary>
    public List<CategoryBreakdownResponse> TopCategoriesByQuantity { get; set; } = [];
}