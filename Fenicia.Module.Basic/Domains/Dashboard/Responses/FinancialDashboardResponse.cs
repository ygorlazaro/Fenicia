namespace Fenicia.Module.Basic.Domains.Dashboard.Responses;

/// <summary>
/// Response model containing comprehensive financial dashboard data.
/// Includes KPIs, revenue vs cost analysis, profit margins, accounts receivable, and daily sales.
/// </summary>
public record FinancialDashboardResponse
{
    /// <summary>Key Performance Indicators summary.</summary>
    public KpiSummaryResponse Kpi { get; set; } = new();
    /// <summary>Revenue vs cost analysis over time.</summary>
    public List<RevenueVsCostResponse> RevenueVsCost { get; set; } = [];
    /// <summary>Profit margin trend over time.</summary>
    public List<ProfitMarginTrendResponse> ProfitMarginTrend { get; set; } = [];
    /// <summary>Accounts receivable summary.</summary>
    public AccountsReceivableResponse AccountsReceivable { get; set; } = new();
    /// <summary>Daily sales summary.</summary>
    public DailySalesSummaryResponse DailySales { get; set; } = new();
}
