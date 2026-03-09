namespace Fenicia.Module.Basic.Domains.Dashboard.GetFinancialDashboard;

public record FinancialDashboardResponse
{
    public KPISummaryResponse KPI { get; set; } = new();
    public List<RevenueVsCostResponse> RevenueVsCost { get; set; } = [];
    public List<ProfitMarginTrendResponse> ProfitMarginTrend { get; set; } = [];
    public AccountsReceivableResponse AccountsReceivable { get; set; } = new();
    public DailySalesSummaryResponse DailySales { get; set; } = new();
}
