using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Enums.Auth;
using Fenicia.Module.Basic.Domains.Dashboard.DTOs;

using Microsoft.EntityFrameworkCore;

using System.Globalization;

namespace Fenicia.Module.Basic.Domains.Dashboard;

public class DashboardService(DefaultContext db)
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

        var todayOrders = db.BasicOrders.Where(o => o.SaleDate.Date == today);
        var weekOrders = db.BasicOrders.Where(o => o.SaleDate.Date >= weekStart);
        var monthOrders = db.BasicOrders.Where(o => o.SaleDate.Date >= monthStart);
        var lastMonthOrders = db.BasicOrders.Where(o => o.SaleDate.Date >= lastMonthStart && o.SaleDate.Date <= lastMonthEnd);

        var todayRevenue = await todayOrders.SumAsync(o => o.TotalAmount, ct);
        var weekRevenue = await weekOrders.SumAsync(o => o.TotalAmount, ct);
        var monthRevenue = await monthOrders.SumAsync(o => o.TotalAmount, ct);
        var lastMonthRevenue = await lastMonthOrders.SumAsync(o => o.TotalAmount, ct);

        var dailySales = new DailySalesSummaryResponse
        {
            TodayRevenue = todayRevenue,
            TodayOrders = await todayOrders.CountAsync(ct),
            WeekRevenue = weekRevenue,
            WeekOrders = await weekOrders.CountAsync(ct),
            MonthRevenue = monthRevenue,
            MonthOrders = await monthOrders.CountAsync(ct),
            PreviousMonthRevenue = lastMonthRevenue,
            GrowthPercentage = lastMonthRevenue > 0 ? (monthRevenue - lastMonthRevenue) / lastMonthRevenue * 100 : 0
        };
        return dailySales;
    }

    private async Task<AccountsReceivableResponse> CalculateAccountsReceivableAsync(CancellationToken ct)
    {
        var pendingOrders = db.BasicOrders.Where(o => o.Status == OrderStatus.Pending);
        var approvedOrders = db.BasicOrders.Where(o => o.Status == OrderStatus.Approved);

        var accountsReceivable = new AccountsReceivableResponse
        {
            TotalPending = await pendingOrders.SumAsync(o => o.TotalAmount,
                ct),
            PendingOrdersCount = await pendingOrders.CountAsync(ct),
            TotalApproved = await approvedOrders.SumAsync(o => o.TotalAmount,
                ct),
            ApprovedOrdersCount = await approvedOrders.CountAsync(ct)
        };

        return accountsReceivable;
    }

    private async Task<List<ProfitMarginTrendResponse>> CalculateProfitMarginTrendAsync(CancellationToken ct)
    {
        var weeks = await GetOrderWeeksAsync(ct);

        var orders = await db.BasicOrders.Select(o => new { o.SaleDate, o.TotalAmount, o.Details }).ToListAsync(ct);

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
        var dates = await GetOrderDatesAsync(ct);

        var response = new List<RevenueVsCostResponse>();

        foreach (var date in dates)
        {
            var key = date.ToString("yyyy MMMM dd");
            var revenue = await db.BasicOrders.SumAsync(o => o.TotalAmount, ct);
            var cost = await db.BasicOrders.SumAsync(o => o.Details.Sum(d => d.Price * (decimal)d.Quantity * 0.7m), ct);
            var profit = revenue - cost;

            response.Add(new RevenueVsCostResponse(key, date, revenue, cost, profit));
        }

        return response;
    }

    private async Task<List<DateTime>> GetOrderDatesAsync(CancellationToken ct)
    {
        var dates = await db.BasicOrders.OrderBy(o => o.SaleDate).Select(o => o.SaleDate.Date).Distinct().ToListAsync(ct);
        return dates;
    }

    private async Task<List<DateTime>> GetOrderWeeksAsync(CancellationToken ct)
    {
        var weeks = await db.BasicOrders.OrderBy(o => o.SaleDate).Select(o => o.SaleDate.Date).Distinct().ToListAsync(ct);

        var weekStarts = new List<DateTime>();
        foreach (var date in weeks)
        {
            var weekStart = date.AddDays(-(int)date.DayOfWeek);
            if (weekStarts.Count == 0 || weekStart > weekStarts[^1])
            {
                weekStarts.Add(weekStart);
            }
        }

        return weekStarts;
    }

    private async Task<KpiSummaryResponse> CalculateKpiSummaryAsync(CancellationToken ct)
    {
        var orders = db.BasicOrders;
        var products = db.BasicProducts;

        var totalRevenue = await orders.SumAsync(o => o.TotalAmount, ct);
        var totalCost = await orders.SumAsync(o => o.Details.Sum(d => d.Price * (decimal)d.Quantity * 0.7m), ct);
        var grossProfit = totalRevenue - totalCost;
        var profitMargin = totalRevenue > 0 ? grossProfit / totalRevenue * 100 : 0;
        var totalOrders = await orders.CountAsync(ct);
        var totalProducts = await products.CountAsync(ct);
        var averageOrderValue = totalOrders > 0 ? totalRevenue / totalOrders : 0;
        var totalStockValue = products.Sum(p => (p.CostPrice ?? 0) * (decimal)p.Quantity);

        var kpi = new KpiSummaryResponse
        {
            TotalRevenue = totalRevenue,
            TotalCost = totalCost,
            GrossProfit = grossProfit,
            ProfitMargin = profitMargin,
            TotalOrders = totalOrders,
            TotalProducts = totalProducts,
            AverageOrderValue = averageOrderValue,
            TotalStockValue = totalStockValue
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
