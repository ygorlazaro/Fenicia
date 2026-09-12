using Fenicia.Common.DTOs.Basic.Dashboard;
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
        DailySalesSummaryResponse dailySales,
        List<CategoryBreakdownResponse> topCategoriesByRevenue,
        List<CategoryBreakdownResponse> topCategoriesByQuantity)
    {
        return new FinancialDashboardResponse
        {
            Kpi = kpi,
            RevenueVsCost = revenueVsCost,
            ProfitMarginTrend = profitMarginTrend,
            AccountsReceivable = accountsReceivable,
            DailySales = dailySales,
            TopCategoriesByRevenue = topCategoriesByRevenue,
            TopCategoriesByQuantity = topCategoriesByQuantity
        };
    }
}