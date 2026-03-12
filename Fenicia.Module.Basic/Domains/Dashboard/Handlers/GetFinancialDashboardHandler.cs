using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Enums.Auth;
using Fenicia.Module.Basic.Domains.Dashboard.Queries;
using Fenicia.Module.Basic.Domains.Dashboard.Responses;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Dashboard.Handlers;

public class GetFinancialDashboardHandler(DefaultContext db)
{
    public async Task<FinancialDashboardResponse> Handle(GetFinancialDashboardQuery query, CancellationToken ct)
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
        var monthStart = new DateTime(today.Year,
            today.Month,
            1);
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
            GrowthPercentage = lastMonthRevenue > 0 ? ((monthRevenue - lastMonthRevenue) / lastMonthRevenue) * 100 : 0
        };
        return dailySales;
    }

    private async Task<AccountsReceivableResponse> CalculateAccountsReceivableAsync(CancellationToken ct)
    {
        var pendingOrders = db.BasicOrders.Where(o => o.Status == OrderStatus.Pending);
        var approvedOrders = db.BasicOrders.Where(o => o.Status == OrderStatus.Approved);

        var accountsReceivable = new AccountsReceivableResponse
        {
            TotalPending = await pendingOrders.SumAsync(o => o.TotalAmount, ct),
            PendingOrdersCount = await pendingOrders.CountAsync(ct),
            TotalApproved = await approvedOrders.SumAsync(o => o.TotalAmount, ct),
            ApprovedOrdersCount = await approvedOrders.CountAsync(ct)
        };
        
        return accountsReceivable;
    }

    private async Task<List<ProfitMarginTrendResponse>> CalculateProfitMarginTrendAsync(CancellationToken ct)
    {
        var request = from o in db.BasicOrders
                        group o by GetWeekNumber(o.SaleDate)
                        into g
                        let Week = g.Min(o => o.SaleDate.Date)
                        let Revenue = g.Sum(o => o.TotalAmount)
                        let Cost = g.Sum(o => o.Details.Sum(d => d.Price * (decimal)d.Quantity * 0.7m))
                        orderby Week
                        select new
                        {
                            Week,
                            Revenue,
                            Cost
                        };

        var weeklyData = await request.ToListAsync(ct);

        var profitMarginTrend = new List<ProfitMarginTrendResponse>();
        
        for (var i = 0; i < weeklyData.Count; i++)
        {
            var current = weeklyData[i];
            var margin = current.Revenue > 0 ? ((current.Revenue - current.Cost) / current.Revenue) * 100 : 0;
        
            var trend = "Stable";
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

        return profitMarginTrend;
    }

    private async Task<List<RevenueVsCostResponse>> CalculateRevenueVsCostAsync(CancellationToken ct)
    {
        var request = from o in db.BasicOrders
                      group o by o.SaleDate.Date
                      into g
                      orderby g.Key.Date descending
                      let revenue = g.Sum(o => o.TotalAmount)
                      let cost = g.Sum(o => o.Details.Sum(d => d.Price * (decimal)d.Quantity * 0.7m))
                      let profit = revenue - cost
                      select new RevenueVsCostResponse(g.Key.ToString("yyyy-MM-dd"),
                          g.Key,
                          revenue,
                          cost,
                          profit);
                      
        return await request.ToListAsync(ct);
    }

    private async Task <KpiSummaryResponse> CalculateKpiSummaryAsync(CancellationToken ct)
    {
        var orders = db.BasicOrders;
        var products = db.BasicProducts;
        
        var totalRevenue = await orders.SumAsync(o => o.TotalAmount, cancellationToken: ct);
        var totalCost = await orders.SumAsync(o => o.Details.Sum(d => d.Price * (decimal)d.Quantity * 0.7m), cancellationToken: ct); // Estimate 70% cost
        var grossProfit = totalRevenue - totalCost;
        var profitMargin = totalRevenue > 0 ? (grossProfit / totalRevenue) * 100 : 0;
        var totalOrders = await orders.CountAsync(cancellationToken: ct);
        var totalProducts = await products.CountAsync(cancellationToken: ct);
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
        var culture = System.Globalization.CultureInfo.CurrentCulture;
        var calendar = culture.Calendar;
        var weekRule = culture.DateTimeFormat.CalendarWeekRule;
        var firstDay = culture.DateTimeFormat.FirstDayOfWeek;
        return calendar.GetWeekOfYear(date,
            weekRule,
            firstDay);
    }
}
