using Fenicia.Common.DTOs.Basic.Dashboard;

namespace Fenicia.Module.Basic.Domains.Dashboard;

/// <summary>
/// Mapper class for mapping dashboard data.
/// </summary>
public static class DashboardMapper
{
    /// <summary>
    /// Maps a financial dashboard request to a FinancialDashboardResponse.
    /// </summary>
    /// <param name="kpi">The KPI summary response.</param>
    /// <param name="revenueVsCost">The revenue vs cost responses.</param>
    /// <param name="profitMarginTrend">The profit margin trend responses.</param>
    /// <param name="accountsReceivable">The accounts receivable response.</param>
    /// <param name="dailySales">The daily sales summary response.</param>
    /// <param name="topCategoriesByRevenue">The top categories by revenue.</param>
    /// <param name="topCategoriesByQuantity">The top categories by quantity.</param>
    /// <returns>The financial dashboard response.</returns>
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