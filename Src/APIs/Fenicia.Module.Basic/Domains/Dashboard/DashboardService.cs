using System.Globalization;

using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Dashboard.DTOs;
using Fenicia.Module.Basic.Domains.Employee;
using Fenicia.Module.Basic.Domains.Order;
using Fenicia.Module.Basic.Domains.Product;

namespace Fenicia.Module.Basic.Domains.Dashboard;

public class DashboardService(
    OrderService orderService,
    ProductService productService,
    EmployeeService employeeService)
{
    public async Task<FinancialDashboardResponse> GetFinancialDashboardAsync(GetFinancialDashboardQuery query, CancellationToken ct)
    {
        var kpi = await CalculateKpiSummaryAsync(ct);
        var revenueVsCost = await CalculateRevenueVsCostAsync(ct);
        var profitMarginTrend = await CalculateProfitMarginTrendAsync(ct);
        var accountsReceivable = await CalculateAccountsReceivableAsync(ct);
        var dailySales = await CalculateDailySalesSummaryAsync(ct);

        return DashboardMapper.MapToFinancialDashboardResponse(kpi, revenueVsCost, profitMarginTrend, accountsReceivable, dailySales);
    }

    public async Task<decimal> GetTotalRevenueAsync(CancellationToken ct)
    {
        return await orderService.GetTotalRevenueAsync(ct);
    }

    public async Task<decimal> GetTotalCostAsync(CancellationToken ct)
    {
        return await orderService.GetTotalCostAsync(ct);
    }

    public async Task<int> GetTotalOrdersAsync(CancellationToken ct)
    {
        return await orderService.GetTotalOrdersCountAsync(ct);
    }

    public async Task<int> GetTotalProductsAsync(CancellationToken ct)
    {
        return await productService.GetTotalProductsAsync(ct);
    }

    public async Task<int> GetTotalEmployeesAsync(CancellationToken ct)
    {
        return await employeeService.GetTotalEmployeesAsync(ct);
    }

    public async Task<List<OrderModel>> GetRecentOrdersAsync(int topLimit, CancellationToken ct)
    {
        return await orderService.GetRecentOrdersAsync(topLimit, ct);
    }

    public async Task<List<OrderModel>> GetTopCustomerOrdersAsync(CancellationToken ct)
    {
        return await orderService.GetTopCustomerOrdersAsync(ct);
    }

    public async Task<List<OrderModel>> GetAtRiskOrdersAsync(CancellationToken ct)
    {
        return await orderService.GetAtRiskOrdersAsync(ct);
    }

    public async Task<List<OrderModel>> GetEmployeePerformanceOrdersAsync(DateTime startDate, DateTime endDate, CancellationToken ct)
    {
        return await orderService.GetEmployeePerformanceOrdersAsync(startDate, endDate, ct);
    }

    public async Task<List<EmployeeModel>> GetAllEmployeesAsync(CancellationToken ct)
    {
        return await employeeService.GetAllEmployeesAsync(ct);
    }

    private static int GetWeekNumber(DateTime date)
    {
        var culture = CultureInfo.CurrentCulture;
        var calendar = culture.Calendar;
        var weekRule = culture.DateTimeFormat.CalendarWeekRule;
        var firstDay = culture.DateTimeFormat.FirstDayOfWeek;
        return calendar.GetWeekOfYear(date, weekRule, firstDay);
    }

    private async Task<DailySalesSummaryResponse> CalculateDailySalesSummaryAsync(CancellationToken ct)
    {
        var today = DateTime.UtcNow.Date;
        var weekStart = today.AddDays(-(int)today.DayOfWeek);
        var monthStart = new DateTime(today.Year, today.Month, 1);
        var lastMonthStart = monthStart.AddMonths(-1);
        var lastMonthEnd = monthStart.AddDays(-1);

        var todayRevenue = await orderService.GetTodayRevenueAsync(ct);
        var weekRevenue = await orderService.GetWeekRevenueAsync(ct);
        var monthRevenue = await orderService.GetMonthRevenueAsync(ct);
        var lastMonthRevenue = await orderService.GetLastMonthRevenueAsync(ct);

        var dailySales = new DailySalesSummaryResponse
        {
            TodayRevenue = todayRevenue,
            TodayOrders = await orderService.GetTodayOrdersCountAsync(ct),
            WeekRevenue = weekRevenue,
            WeekOrders = await orderService.GetWeekOrdersCountAsync(ct),
            MonthRevenue = monthRevenue,
            MonthOrders = await orderService.GetMonthOrdersCountAsync(ct),
            PreviousMonthRevenue = lastMonthRevenue,
            GrowthPercentage = lastMonthRevenue > 0 ? (monthRevenue - lastMonthRevenue) / lastMonthRevenue * 100 : 0
        };
        return dailySales;
    }

    private async Task<AccountsReceivableResponse> CalculateAccountsReceivableAsync(CancellationToken ct)
    {
        var accountsReceivable = new AccountsReceivableResponse
        {
            TotalPending = await orderService.GetPendingAmountAsync(ct),
            PendingOrdersCount = await orderService.GetPendingOrdersCountAsync(ct),
            TotalApproved = await orderService.GetApprovedAmountAsync(ct),
            ApprovedOrdersCount = await orderService.GetApprovedOrdersCountAsync(ct)
        };

        return accountsReceivable;
    }

    private async Task<List<ProfitMarginTrendResponse>> CalculateProfitMarginTrendAsync(CancellationToken ct)
    {
        var weeks = await orderService.GetOrderWeeksAsync(ct);
        var orders = await orderService.GetTopCustomerOrdersAsync(ct);

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
        var dates = await orderService.GetOrderDatesAsync(ct);

        var response = new List<RevenueVsCostResponse>();

        foreach (var date in dates)
        {
            var key = date.ToString("yyyy MMMM dd");
            var revenue = await orderService.GetTotalRevenueAsync(ct);
            var cost = await orderService.GetTotalCostAsync(ct);
            var profit = revenue - cost;

            response.Add(new RevenueVsCostResponse(key, date, revenue, cost, profit));
        }

        return response;
    }

    private async Task<KpiSummaryResponse> CalculateKpiSummaryAsync(CancellationToken ct)
    {
        var totalRevenue = await orderService.GetTotalRevenueAsync(ct);
        var totalCost = await orderService.GetTotalCostAsync(ct);
        var grossProfit = totalRevenue - totalCost;
        var profitMargin = totalRevenue > 0 ? grossProfit / totalRevenue * 100 : 0;
        var totalOrders = await orderService.GetTotalOrdersCountAsync(ct);
        var totalProducts = await productService.GetTotalProductsAsync(ct);
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
}
