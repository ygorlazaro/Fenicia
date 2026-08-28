namespace Fenicia.Module.Basic.Domains.Dashboard.DTOs;

public record FinancialDashboardResponse
{

    public KpiSummaryResponse Kpi { get; set; } = new();

    public List<RevenueVsCostResponse> RevenueVsCost { get; set; } = [];

    public List<ProfitMarginTrendResponse> ProfitMarginTrend { get; set; } = [];

    public AccountsReceivableResponse AccountsReceivable { get; set; } = new();

    public DailySalesSummaryResponse DailySales { get; set; } = new();
}
