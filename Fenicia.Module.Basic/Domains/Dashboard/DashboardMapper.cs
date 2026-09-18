using Fenicia.Common.DTOs.Basic.Dashboard;
using Riok.Mapperly.Abstractions;

namespace Fenicia.Module.Basic.Domains.Dashboard;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
public partial class DashboardMapper
{
    internal partial FinancialDashboardResponse MapToFinancialDashboardResponse(
        KpiSummaryResponse kpi,
        List<RevenueVsCostResponse> revenueVsCost,
        List<ProfitMarginTrendResponse> profitMarginTrend,
        AccountsReceivableResponse accountsReceivable,
        DailySalesSummaryResponse dailySales,
        List<CategoryBreakdownResponse> topCategoriesByRevenue,
        List<CategoryBreakdownResponse> topCategoriesByQuantity);
}
