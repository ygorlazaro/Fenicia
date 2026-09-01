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
    public virtual async Task<FinancialDashboardResponse> GetFinancialDashboardAsync(GetFinancialDashboardQuery query, CancellationToken cancellationToken = default)
    {
        var kpi = await CalculateKpiSummaryAsync(cancellationToken);
        var revenueVsCost = await CalculateRevenueVsCostAsync(cancellationToken);
        var profitMarginTrend = await CalculateProfitMarginTrendAsync(cancellationToken);
        var accountsReceivable = await CalculateAccountsReceivableAsync(cancellationToken);
        var dailySales = await CalculateDailySalesSummaryAsync(cancellationToken);

        return DashboardMapper.MapToFinancialDashboardResponse(kpi, revenueVsCost, profitMarginTrend, accountsReceivable, dailySales);
    }

    public virtual async Task<decimal> GetTotalRevenueAsync(CancellationToken cancellationToken = default)
    {
        return await orderService.GetTotalRevenueAsync(cancellationToken);
    }

    public virtual async Task<decimal> GetTotalCostAsync(CancellationToken cancellationToken = default)
    {
        return await orderService.GetTotalCostAsync(cancellationToken);
    }

    public virtual async Task<int> GetTotalOrdersAsync(CancellationToken cancellationToken = default)
    {
        return await orderService.GetTotalOrdersCountAsync(cancellationToken);
    }

    public virtual async Task<int> GetTotalProductsAsync(CancellationToken cancellationToken = default)
    {
        return await productService.GetTotalProductsAsync(cancellationToken);
    }

    public virtual async Task<int> GetTotalEmployeesAsync(CancellationToken cancellationToken = default)
    {
        return await employeeService.GetTotalEmployeesAsync(cancellationToken);
    }

    public virtual async Task<List<OrderModel>> GetRecentOrdersAsync(int topLimit, CancellationToken cancellationToken = default)
    {
        return await orderService.GetRecentOrdersAsync(topLimit, cancellationToken);
    }

    public virtual async Task<List<OrderModel>> GetTopCustomerOrdersAsync(CancellationToken cancellationToken = default)
    {
        return await orderService.GetTopCustomerOrdersAsync(cancellationToken);
    }

    public virtual async Task<List<OrderModel>> GetAtRiskOrdersAsync(CancellationToken cancellationToken = default)
    {
        return await orderService.GetAtRiskOrdersAsync(cancellationToken);
    }

    public virtual async Task<List<OrderModel>> GetEmployeePerformanceOrdersAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await orderService.GetEmployeePerformanceOrdersAsync(startDate, endDate, cancellationToken);
    }

    public virtual async Task<List<EmployeeModel>> GetAllEmployeesAsync(CancellationToken cancellationToken = default)
    {
        return await employeeService.GetAllEmployeesAsync(cancellationToken);
    }

    private static int GetWeekNumber(DateTime date)
    {
        var culture = CultureInfo.CurrentCulture;
        var calendar = culture.Calendar;
        var weekRule = culture.DateTimeFormat.CalendarWeekRule;
        var firstDay = culture.DateTimeFormat.FirstDayOfWeek;
        return calendar.GetWeekOfYear(date, weekRule, firstDay);
    }

    private async Task<DailySalesSummaryResponse> CalculateDailySalesSummaryAsync(CancellationToken cancellationToken = default)
    {
        var todayRevenue = await orderService.GetTodayRevenueAsync(cancellationToken);
        var weekRevenue = await orderService.GetWeekRevenueAsync(cancellationToken);
        var monthRevenue = await orderService.GetMonthRevenueAsync(cancellationToken);
        var lastMonthRevenue = await orderService.GetLastMonthRevenueAsync(cancellationToken);

        var dailySales = new DailySalesSummaryResponse
        {
            TodayRevenue = todayRevenue,
            TodayOrders = await orderService.GetTodayOrdersCountAsync(cancellationToken),
            WeekRevenue = weekRevenue,
            WeekOrders = await orderService.GetWeekOrdersCountAsync(cancellationToken),
            MonthRevenue = monthRevenue,
            MonthOrders = await orderService.GetMonthOrdersCountAsync(cancellationToken),
            PreviousMonthRevenue = lastMonthRevenue,
            GrowthPercentage = lastMonthRevenue > 0 ? (monthRevenue - lastMonthRevenue) / lastMonthRevenue * 100 : 0
        };
        return dailySales;
    }

    private async Task<AccountsReceivableResponse> CalculateAccountsReceivableAsync(CancellationToken cancellationToken = default)
    {
        var accountsReceivable = new AccountsReceivableResponse
        {
            TotalPending = await orderService.GetPendingAmountAsync(cancellationToken),
            PendingOrdersCount = await orderService.GetPendingOrdersCountAsync(cancellationToken),
            TotalApproved = await orderService.GetApprovedAmountAsync(cancellationToken),
            ApprovedOrdersCount = await orderService.GetApprovedOrdersCountAsync(cancellationToken)
        };

        return accountsReceivable;
    }

    private async Task<List<ProfitMarginTrendResponse>> CalculateProfitMarginTrendAsync(CancellationToken cancellationToken = default)
    {
        var weeks = await orderService.GetOrderWeeksAsync(cancellationToken);
        var orders = await orderService.GetTopCustomerOrdersAsync(cancellationToken);

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

    private async Task<List<RevenueVsCostResponse>> CalculateRevenueVsCostAsync(CancellationToken cancellationToken = default)
    {
        var dates = await orderService.GetOrderDatesAsync(cancellationToken);

        var response = new List<RevenueVsCostResponse>();

        foreach (var date in dates)
        {
            var key = date.ToString("yyyy MMMM dd");
            var revenue = await orderService.GetTotalRevenueAsync(cancellationToken);
            var cost = await orderService.GetTotalCostAsync(cancellationToken);
            var profit = revenue - cost;

            response.Add(new RevenueVsCostResponse(key, date, revenue, cost, profit));
        }

        return response;
    }

    private async Task<KpiSummaryResponse> CalculateKpiSummaryAsync(CancellationToken cancellationToken = default)
    {
        var totalRevenue = await orderService.GetTotalRevenueAsync(cancellationToken);
        var totalCost = await orderService.GetTotalCostAsync(cancellationToken);
        var grossProfit = totalRevenue - totalCost;
        var profitMargin = totalRevenue > 0 ? grossProfit / totalRevenue * 100 : 0;
        var totalOrders = await orderService.GetTotalOrdersCountAsync(cancellationToken);
        var totalProducts = await productService.GetTotalProductsAsync(cancellationToken);
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
