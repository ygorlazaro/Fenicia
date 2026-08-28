using Fenicia.Common.Enums.Auth;
using Fenicia.Module.Basic.Domains.Dashboard;
using Fenicia.Module.Basic.Domains.Dashboard.DTOs;

using System.Globalization;

namespace Fenicia.Module.Basic.Domains.Dashboard;

public class DashboardService(DashboardRepository dashboardRepository)
{
    public async Task<FinancialDashboardResponse> GetFinancialDashboardAsync(GetFinancialDashboardQuery query, CancellationToken ct)
    {
        var kpi = await CalculateKpiSummaryAsync(ct);
        var revenueVsCost = await CalculateRevenueVsCostAsync(ct);
        var profitMarginTrend = await CalculateProfitMarginTrendAsync(ct);
        var accountsReceivable = await CalculateAccountsReceivableAsync(ct);
        var dailySales = await CalculateDailySalesSummaryAsync(ct);

        return new FinancialDashboardResponse
        {
            Kpi = kpi,
            RevenueVsCost = revenueVsCost,
            ProfitMarginTrend = profitMarginTrend,
            AccountsReceivable = accountsReceivable,
            DailySales = dailySales
        };
    }

    private async Task<DailySalesSummaryResponse> CalculateDailySalesSummaryAsync(CancellationToken ct)
    {
        var today = DateTime.UtcNow.Date;
        var weekStart = today.AddDays(-(int)today.DayOfWeek);
        var monthStart = new DateTime(today.Year, today.Month, 1);
        var lastMonthStart = monthStart.AddMonths(-1);
        var lastMonthEnd = monthStart.AddDays(-1);

        var todayRevenue = await dashboardRepository.GetTodayRevenueAsync(ct);
        var weekRevenue = await dashboardRepository.GetWeekRevenueAsync(ct);
        var monthRevenue = await dashboardRepository.GetMonthRevenueAsync(ct);
        var lastMonthRevenue = await dashboardRepository.GetLastMonthRevenueAsync(ct);

        var dailySales = new DailySalesSummaryResponse
        {
            TodayRevenue = todayRevenue,
            TodayOrders = await dashboardRepository.GetTodayOrdersAsync(ct),
            WeekRevenue = weekRevenue,
            WeekOrders = await dashboardRepository.GetWeekOrdersAsync(ct),
            MonthRevenue = monthRevenue,
            MonthOrders = await dashboardRepository.GetMonthOrdersAsync(ct),
            PreviousMonthRevenue = lastMonthRevenue,
            GrowthPercentage = lastMonthRevenue > 0 ? (monthRevenue - lastMonthRevenue) / lastMonthRevenue * 100 : 0
        };
        return dailySales;
    }

    private async Task<AccountsReceivableResponse> CalculateAccountsReceivableAsync(CancellationToken ct)
    {
        var accountsReceivable = new AccountsReceivableResponse
        {
            TotalPending = await dashboardRepository.GetPendingAmountAsync(ct),
            PendingOrdersCount = await dashboardRepository.GetPendingOrdersCountAsync(ct),
            TotalApproved = await dashboardRepository.GetApprovedAmountAsync(ct),
            ApprovedOrdersCount = await dashboardRepository.GetApprovedOrdersCountAsync(ct)
        };

        return accountsReceivable;
    }

    private async Task<List<ProfitMarginTrendResponse>> CalculateProfitMarginTrendAsync(CancellationToken ct)
    {
        var weeks = await dashboardRepository.GetOrderWeeksAsync(ct);
        var orders = await dashboardRepository.GetTopCustomerOrdersAsync(ct);

        var response = new List<ProfitMarginTrendResponse>();

        foreach (var week in weeks)
        {
            var weekNumber = GetWeekNumber(week);
            var weekOrders = orders.Where(o => GetWeekNumber(o.SaleDate) == weekNumber).ToList();

            var revenue = weekOrders.Sum(o => o.TotalAmount);
            var cost = weekOrders.Sum(o => o.Details.Sum(d => d.Price * (decimal)d.Quantity * 0.7m));

            var margin = revenue > 0 ? (revenue - cost) / revenue * 100 : 0;

            var trend = "Stable";
            if (response.Count > 0)
            {
                var previous = response[^1];
                var prevMargin = previous.MarginPercentage;
                trend = margin > prevMargin + 2 ? "Improving" : margin < prevMargin - 2 ? "Declining" : "Stable";
            }

            response.Add(new ProfitMarginTrendResponse($"Week {weekNumber}", week, margin, trend));
        }

        return response;
    }

    private async Task<List<RevenueVsCostResponse>> CalculateRevenueVsCostAsync(CancellationToken ct)
    {
        var dates = await dashboardRepository.GetOrderDatesAsync(ct);

        var response = new List<RevenueVsCostResponse>();

        foreach (var date in dates)
        {
            var key = date.ToString("yyyy MMMM dd");
            var revenue = await dashboardRepository.GetTotalRevenueAsync(ct);
            var cost = await dashboardRepository.GetTotalCostAsync(ct);
            var profit = revenue - cost;

            response.Add(new RevenueVsCostResponse(key, date, revenue, cost, profit));
        }

        return response;
    }

    private async Task<KpiSummaryResponse> CalculateKpiSummaryAsync(CancellationToken ct)
    {
        var totalRevenue = await dashboardRepository.GetTotalRevenueAsync(ct);
        var totalCost = await dashboardRepository.GetTotalCostAsync(ct);
        var grossProfit = totalRevenue - totalCost;
        var profitMargin = totalRevenue > 0 ? grossProfit / totalRevenue * 100 : 0;
        var totalOrders = await dashboardRepository.GetTotalOrdersAsync(ct);
        var totalProducts = await dashboardRepository.GetTotalProductsAsync(ct);
        var averageOrderValue = totalOrders > 0 ? totalRevenue / totalOrders : 0;

        var kpi = new KpiSummaryResponse
        {
            TotalRevenue = totalRevenue,
            TotalCost = totalCost,
            GrossProfit = grossProfit,
            ProfitMargin = profitMargin,
            TotalOrders = totalOrders,
            TotalProducts = totalProducts,
            AverageOrderValue = averageOrderValue,
            TotalStockValue = 0
        };
        return kpi;
    }

    private static int GetWeekNumber(DateTime date)
    {
        var culture = CultureInfo.CurrentCulture;
        var calendar = culture.Calendar;
        var weekRule = culture.DateTimeFormat.CalendarWeekRule;
        var firstDay = culture.DateTimeFormat.FirstDayOfWeek;
        return calendar.GetWeekOfYear(date, weekRule, firstDay);
    }
}
