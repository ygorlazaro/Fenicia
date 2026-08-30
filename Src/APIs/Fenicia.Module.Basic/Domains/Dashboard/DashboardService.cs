using System.Globalization;

using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Dashboard.DTOs;
using Fenicia.Module.Basic.Domains.Employee;
using Fenicia.Module.Basic.Domains.Order;
using Fenicia.Module.Basic.Domains.Product;

namespace Fenicia.Module.Basic.Domains.Dashboard;

public class DashboardService
{
    private readonly OrderService _orderService;
    private readonly ProductService _productService;
    private readonly EmployeeService _employeeService;

    public DashboardService()
        : this(null!, null!, null!)
    {
    }

    public DashboardService(
        OrderService orderService,
        ProductService productService,
        EmployeeService employeeService)
    {
        _orderService = orderService;
        _productService = productService;
        _employeeService = employeeService;
    }

    public virtual async Task<FinancialDashboardResponse> GetFinancialDashboardAsync(GetFinancialDashboardQuery query, CancellationToken ct)
    {
        var kpi = await CalculateKpiSummaryAsync(ct);
        var revenueVsCost = await CalculateRevenueVsCostAsync(ct);
        var profitMarginTrend = await CalculateProfitMarginTrendAsync(ct);
        var accountsReceivable = await CalculateAccountsReceivableAsync(ct);
        var dailySales = await CalculateDailySalesSummaryAsync(ct);

        return DashboardMapper.MapToFinancialDashboardResponse(kpi, revenueVsCost, profitMarginTrend, accountsReceivable, dailySales);
    }

    public virtual async Task<decimal> GetTotalRevenueAsync(CancellationToken ct)
    {
        return await _orderService.GetTotalRevenueAsync(ct);
    }

    public virtual async Task<decimal> GetTotalCostAsync(CancellationToken ct)
    {
        return await _orderService.GetTotalCostAsync(ct);
    }

    public virtual async Task<int> GetTotalOrdersAsync(CancellationToken ct)
    {
        return await _orderService.GetTotalOrdersCountAsync(ct);
    }

    public virtual async Task<int> GetTotalProductsAsync(CancellationToken ct)
    {
        return await _productService.GetTotalProductsAsync(ct);
    }

    public virtual async Task<int> GetTotalEmployeesAsync(CancellationToken ct)
    {
        return await _employeeService.GetTotalEmployeesAsync(ct);
    }

    public virtual async Task<List<OrderModel>> GetRecentOrdersAsync(int topLimit, CancellationToken ct)
    {
        return await _orderService.GetRecentOrdersAsync(topLimit, ct);
    }

    public virtual async Task<List<OrderModel>> GetTopCustomerOrdersAsync(CancellationToken ct)
    {
        return await _orderService.GetTopCustomerOrdersAsync(ct);
    }

    public virtual async Task<List<OrderModel>> GetAtRiskOrdersAsync(CancellationToken ct)
    {
        return await _orderService.GetAtRiskOrdersAsync(ct);
    }

    public virtual async Task<List<OrderModel>> GetEmployeePerformanceOrdersAsync(DateTime startDate, DateTime endDate, CancellationToken ct)
    {
        return await _orderService.GetEmployeePerformanceOrdersAsync(startDate, endDate, ct);
    }

    public virtual async Task<List<EmployeeModel>> GetAllEmployeesAsync(CancellationToken ct)
    {
        return await _employeeService.GetAllEmployeesAsync(ct);
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

        var todayRevenue = await _orderService.GetTodayRevenueAsync(ct);
        var weekRevenue = await _orderService.GetWeekRevenueAsync(ct);
        var monthRevenue = await _orderService.GetMonthRevenueAsync(ct);
        var lastMonthRevenue = await _orderService.GetLastMonthRevenueAsync(ct);

        var dailySales = new DailySalesSummaryResponse
        {
            TodayRevenue = todayRevenue,
            TodayOrders = await _orderService.GetTodayOrdersCountAsync(ct),
            WeekRevenue = weekRevenue,
            WeekOrders = await _orderService.GetWeekOrdersCountAsync(ct),
            MonthRevenue = monthRevenue,
            MonthOrders = await _orderService.GetMonthOrdersCountAsync(ct),
            PreviousMonthRevenue = lastMonthRevenue,
            GrowthPercentage = lastMonthRevenue > 0 ? (monthRevenue - lastMonthRevenue) / lastMonthRevenue * 100 : 0
        };
        return dailySales;
    }

    private async Task<AccountsReceivableResponse> CalculateAccountsReceivableAsync(CancellationToken ct)
    {
        var accountsReceivable = new AccountsReceivableResponse
        {
            TotalPending = await _orderService.GetPendingAmountAsync(ct),
            PendingOrdersCount = await _orderService.GetPendingOrdersCountAsync(ct),
            TotalApproved = await _orderService.GetApprovedAmountAsync(ct),
            ApprovedOrdersCount = await _orderService.GetApprovedOrdersCountAsync(ct)
        };

        return accountsReceivable;
    }

    private async Task<List<ProfitMarginTrendResponse>> CalculateProfitMarginTrendAsync(CancellationToken ct)
    {
        var weeks = await _orderService.GetOrderWeeksAsync(ct);
        var orders = await _orderService.GetTopCustomerOrdersAsync(ct);

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
        var dates = await _orderService.GetOrderDatesAsync(ct);

        var response = new List<RevenueVsCostResponse>();

        foreach (var date in dates)
        {
            var key = date.ToString("yyyy MMMM dd");
            var revenue = await _orderService.GetTotalRevenueAsync(ct);
            var cost = await _orderService.GetTotalCostAsync(ct);
            var profit = revenue - cost;

            response.Add(new RevenueVsCostResponse(key, date, revenue, cost, profit));
        }

        return response;
    }

    private async Task<KpiSummaryResponse> CalculateKpiSummaryAsync(CancellationToken ct)
    {
        var totalRevenue = await _orderService.GetTotalRevenueAsync(ct);
        var totalCost = await _orderService.GetTotalCostAsync(ct);
        var grossProfit = totalRevenue - totalCost;
        var profitMargin = totalRevenue > 0 ? grossProfit / totalRevenue * 100 : 0;
        var totalOrders = await _orderService.GetTotalOrdersCountAsync(ct);
        var totalProducts = await _productService.GetTotalProductsAsync(ct);
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
