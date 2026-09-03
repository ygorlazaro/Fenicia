using Fenicia.Module.Basic.Domains.Dashboard.DTOs;
using Riok.Mapperly.Abstractions;

namespace Fenicia.Module.Basic.Domains.Dashboard;

[Mapper]
public static partial class DashboardMapper
{
    public static FinancialDashboardResponse MapToFinancialDashboardResponse(
        KpiSummaryResponse kpi,
        List<RevenueVsCostResponse> revenueVsCost,
        List<ProfitMarginTrendResponse> profitMarginTrend,
        AccountsReceivableResponse accountsReceivable,
        DailySalesSummaryResponse dailySales)
    {
        return new FinancialDashboardResponse
        {
            Kpi = kpi,
            RevenueVsCost = revenueVsCost,
            ProfitMarginTrend = profitMarginTrend,
            AccountsReceivable = accountsReceivable,
            DailySales = dailySales
        };
    }
}