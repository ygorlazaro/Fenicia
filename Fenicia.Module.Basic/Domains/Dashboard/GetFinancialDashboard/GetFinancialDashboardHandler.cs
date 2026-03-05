using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Enums.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Dashboard.GetFinancialDashboard;

public class GetFinancialDashboardHandler(DefaultContext context)
{
    public async Task<FinancialDashboardResponse> Handle(GetFinancialDashboardQuery query, CancellationToken ct)
    {
        var endDate = DateTime.UtcNow;
        var startDate = endDate.AddDays(-query.Days);

        var orders = await context.BasicOrders
            .Include(o => o.Details)
            .Where(o => o.SaleDate >= startDate && o.SaleDate <= endDate)
            .ToListAsync(ct);

        var products = await context.BasicProducts
            .Include(p => p.Category)
            .ToListAsync(ct);

        var allOrders = await context.BasicOrders
            .Include(o => o.Details)
            .ToListAsync(ct);

        // 1. KPI Summary
        var totalRevenue = orders.Sum(o => o.TotalAmount);
        var totalCost = orders.Sum(o => o.Details.Sum(d => d.Price * (decimal)d.Quantity * 0.7m)); // Estimate 70% cost
        var grossProfit = totalRevenue - totalCost;
        var profitMargin = totalRevenue > 0 ? (grossProfit / totalRevenue) * 100 : 0;

        var kpi = new KPISummaryResponse
        {
            TotalRevenue = totalRevenue,
            TotalCost = totalCost,
            GrossProfit = grossProfit,
            ProfitMargin = profitMargin,
            TotalOrders = orders.Count,
            TotalProducts = products.Count,
            AverageOrderValue = orders.Count > 0 ? totalRevenue / orders.Count : 0,
            TotalStockValue = products.Sum(p => (p.CostPrice ?? 0) * (decimal)p.Quantity)
        };

        // 2. Revenue vs Cost Over Time (daily)
        var revenueVsCost = orders
            .GroupBy(o => o.SaleDate.Date)
            .Select(g => new RevenueVsCostResponse(
                g.Key.ToString("yyyy-MM-dd"),
                g.Key,
                g.Sum(o => o.TotalAmount),
                g.Sum(o => o.Details.Sum(d => d.Price * (decimal)d.Quantity * 0.7m)),
                g.Sum(o => o.TotalAmount) - g.Sum(o => o.Details.Sum(d => d.Price * (decimal)d.Quantity * 0.7m))))
            .OrderBy(r => r.Date)
            .ToList();

        // 3. Profit Margin Trend (weekly)
        var weeklyData = orders
            .GroupBy(o => GetWeekNumber(o.SaleDate))
            .Select(g => new
            {
                Week = g.Min(o => o.SaleDate.Date),
                Revenue = g.Sum(o => o.TotalAmount),
                Cost = g.Sum(o => o.Details.Sum(d => d.Price * (decimal)d.Quantity * 0.7m))
            })
            .OrderBy(w => w.Week)
            .ToList();

        var profitMarginTrend = new List<ProfitMarginTrendResponse>();
        for (var i = 0; i < weeklyData.Count; i++)
        {
            var current = weeklyData[i];
            var margin = current.Revenue > 0 ? ((current.Revenue - current.Cost) / current.Revenue) * 100 : 0;
            
            string trend = "Stable";
            if (i > 0)
            {
                var previous = weeklyData[i - 1];
                var prevMargin = previous.Revenue > 0 ? ((previous.Revenue - previous.Cost) / previous.Revenue) * 100 : 0;
                trend = margin > prevMargin + 2 ? "Improving" : margin < prevMargin - 2 ? "Declining" : "Stable";
            }

            profitMarginTrend.Add(new ProfitMarginTrendResponse(
                $"Week {GetWeekNumber(current.Week)}",
                current.Week,
                margin,
                trend));
        }

        // 4. Accounts Receivable (Pending vs Approved orders)
        var pendingOrders = allOrders.Where(o => o.Status == OrderStatus.Pending).ToList();
        var approvedOrders = allOrders.Where(o => o.Status == OrderStatus.Approved).ToList();

        var accountsReceivable = new AccountsReceivableResponse
        {
            TotalPending = pendingOrders.Sum(o => o.TotalAmount),
            PendingOrdersCount = pendingOrders.Count,
            TotalApproved = approvedOrders.Sum(o => o.TotalAmount),
            ApprovedOrdersCount = approvedOrders.Count
        };

        // 5. Daily Sales Summary
        var today = DateTime.UtcNow.Date;
        var weekStart = today.AddDays(-(int)today.DayOfWeek);
        var monthStart = new DateTime(today.Year, today.Month, 1);
        var lastMonthStart = monthStart.AddMonths(-1);
        var lastMonthEnd = monthStart.AddDays(-1);

        var todayOrders = allOrders.Where(o => o.SaleDate.Date == today).ToList();
        var weekOrders = allOrders.Where(o => o.SaleDate.Date >= weekStart).ToList();
        var monthOrders = allOrders.Where(o => o.SaleDate.Date >= monthStart).ToList();
        var lastMonthOrders = allOrders.Where(o => o.SaleDate.Date >= lastMonthStart && o.SaleDate.Date <= lastMonthEnd).ToList();

        var todayRevenue = todayOrders.Sum(o => o.TotalAmount);
        var weekRevenue = weekOrders.Sum(o => o.TotalAmount);
        var monthRevenue = monthOrders.Sum(o => o.TotalAmount);
        var lastMonthRevenue = lastMonthOrders.Sum(o => o.TotalAmount);

        var dailySales = new DailySalesSummaryResponse
        {
            TodayRevenue = todayRevenue,
            TodayOrders = todayOrders.Count,
            WeekRevenue = weekRevenue,
            WeekOrders = weekOrders.Count,
            MonthRevenue = monthRevenue,
            MonthOrders = monthOrders.Count,
            PreviousMonthRevenue = lastMonthRevenue,
            GrowthPercentage = lastMonthRevenue > 0 ? ((monthRevenue - lastMonthRevenue) / lastMonthRevenue) * 100 : 0
        };

        return new FinancialDashboardResponse
        {
            KPI = kpi,
            RevenueVsCost = revenueVsCost,
            ProfitMarginTrend = profitMarginTrend,
            AccountsReceivable = accountsReceivable,
            DailySales = dailySales
        };
    }

    private static int GetWeekNumber(DateTime date)
    {
        var culture = System.Globalization.CultureInfo.CurrentCulture;
        var calendar = culture.Calendar;
        var weekRule = culture.DateTimeFormat.CalendarWeekRule;
        var firstDay = culture.DateTimeFormat.FirstDayOfWeek;
        return calendar.GetWeekOfYear(date, weekRule, firstDay);
    }
}
