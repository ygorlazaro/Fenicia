namespace Fenicia.Common.DTOs.Basic.Dashboard;

public class FinancialDashboardResponse
{
    public KpiSummaryResponse Kpi { get; set; } = new();

    public List<RevenueVsCostResponse> RevenueVsCost { get; set; } = [];

    public List<ProfitMarginTrendResponse> ProfitMarginTrend { get; set; } = [];

    public AccountsReceivableResponse AccountsReceivable { get; set; } = new();

    public DailySalesSummaryResponse DailySales { get; set; } = new();

    public List<CategoryBreakdownResponse> TopCategoriesByRevenue { get; set; } = [];

    public List<CategoryBreakdownResponse> TopCategoriesByQuantity { get; set; } = [];
}
