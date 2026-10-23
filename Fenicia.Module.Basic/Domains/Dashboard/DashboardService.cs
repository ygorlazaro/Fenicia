using System.Globalization;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.DTOs.Basic.Dashboard;
using Fenicia.Common.DTOs.Basic.Order;
using Fenicia.Common.Enums.Auth;
using Fenicia.Module.Basic.Domains.Dashboard.Interfaces;
using Fenicia.Module.Basic.Domains.Employee.Interfaces;
using Fenicia.Module.Basic.Domains.Order.Interfaces;
using Fenicia.Module.Basic.Domains.Product.Interfaces;

namespace Fenicia.Module.Basic.Domains.Dashboard;

/// <summary>
/// Service implementation for managing dashboard data.
/// </summary>
/// <param name="orderService">The order service.</param>
/// <param name="productService">The product service.</param>
/// <param name="employeeService">The employee service.</param>
public sealed class DashboardService(
    IOrderService orderService,
    IProductService productService,
    IEmployeeService employeeService) : IDashboardService
{
    /// <inheritdoc />
    public async Task<FinancialDashboardResponse> GetFinancialDashboardAsync(
        FinancialDashboardRequest request,
        CancellationToken cancellationToken = default)
    {
        var kpi = await CalculateKpiSummaryAsync(request.Days, cancellationToken);
        var revenueVsCost = await CalculateRevenueVsCostAsync(request.Days, cancellationToken);
        var profitMarginTrend = await CalculateProfitMarginTrendAsync(request.Days, cancellationToken);
        var accountsReceivable = await CalculateAccountsReceivableAsync(request.Days, cancellationToken);
        var dailySales = await CalculateDailySalesSummaryAsync(cancellationToken);
        var topCategoriesByRevenue = await CalculateTopCategoriesByRevenueAsync(request.Days, cancellationToken);
        var topCategoriesByQuantity = await CalculateTopCategoriesByQuantityAsync(request.Days, cancellationToken);

        return DashboardMapper.MapToFinancialDashboardResponse(
            kpi,
            revenueVsCost,
            profitMarginTrend,
            accountsReceivable,
            dailySales,
            topCategoriesByRevenue,
            topCategoriesByQuantity);
    }

    /// <inheritdoc />
    public Task<decimal> GetTotalCostAsync(CancellationToken cancellationToken = default)
    {
        return orderService.GetTotalCostAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<int> GetTotalProductsAsync(CancellationToken cancellationToken = default)
    {
        return productService.GetTotalProductsAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<int> GetTotalEmployeesAsync(CancellationToken cancellationToken = default)
    {
        return employeeService.GetTotalEmployeesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<List<OrderResponse>> GetRecentOrdersAsync(
        int topLimit,
        CancellationToken cancellationToken = default)
    {
        return orderService.GetRecentOrdersAsync(topLimit, cancellationToken);
    }

    /// <inheritdoc />
    public Task<List<OrderResponse>> GetTopCustomerOrdersAsync(CancellationToken cancellationToken = default)
    {
        return orderService.GetTopCustomerOrdersAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<List<OrderResponse>> GetAtRiskOrdersAsync(CancellationToken cancellationToken = default)
    {
        return orderService.GetAtRiskOrdersAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<List<OrderResponse>> GetEmployeePerformanceOrdersAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        return orderService.GetEmployeePerformanceOrdersAsync(startDate, endDate, cancellationToken);
    }

    /// <inheritdoc />
    public Task<List<EmployeeModel>> GetAllEmployeesAsync(CancellationToken cancellationToken = default)
    {
        return employeeService.GetAllEmployeesAsync(cancellationToken);
    }

    /// <summary>
    /// Gets the week number for the specified date.
    /// </summary>
    /// <param name="date">The date to get the week number for.</param>
    /// <returns>The week number.</returns>
    private static int GetWeekNumber(DateTime date)
    {
        var culture = CultureInfo.CurrentCulture;
        var calendar = culture.Calendar;
        var weekRule = culture.DateTimeFormat.CalendarWeekRule;
        var firstDay = culture.DateTimeFormat.FirstDayOfWeek;
        return calendar.GetWeekOfYear(date, weekRule, firstDay);
    }

    /// <summary>
    /// Builds a top categories list with an "Others" aggregation.
    /// </summary>
    /// <param name="categories">The list of categories ordered by value.</param>
    /// <param name="topCount">The number of top categories to keep.</param>
    /// <returns>A list with top categories and an aggregated "Others" category if needed.</returns>
    private static List<CategoryBreakdownResponse> BuildTopWithOthers(
        List<CategoryBreakdownResponse> categories,
        int topCount)
    {
        if (categories.Count == 0)
        {
            return categories;
        }

        var top = categories.Take(topCount).ToList();
        var others = categories.Skip(topCount).ToList();

        if (others.Count <= 0)
        {
            return top;
        }

        var otherRevenue = others.Sum(c => c.Revenue);
        var otherQuantity = others.Sum(c => c.Quantity);
        top.Add(
            new CategoryBreakdownResponse
            {
                Category = "Outros",
                Revenue = otherRevenue,
                Quantity = otherQuantity,
                IsOther = true
            });

        return top;
    }

    /// <summary>
    /// Calculates the top categories by revenue for the specified period.
    /// </summary>
    /// <param name="days">The number of days to look back.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of category breakdown responses ordered by revenue.</returns>
    private async Task<List<CategoryBreakdownResponse>> CalculateTopCategoriesByRevenueAsync(
        int days,
        CancellationToken cancellationToken = default)
    {
        var endDate = DateTime.UtcNow;
        var startDate = endDate.AddDays(-days);
        var orders = await orderService.GetAnalyticsOrdersAsync(startDate, endDate, cancellationToken);
        var orderList = orders.ToList();

        var categoryRevenue = orderList
            .SelectMany(o => o.Details)
            .GroupBy(d => d.Product.Category.Name)
            .Select(g => new CategoryBreakdownResponse
            {
                Category = g.Key,
                Revenue = g.Sum(d => d.Price * (decimal)d.Quantity),
                Quantity = g.Sum(d => d.Quantity),
                IsOther = false
            })
            .OrderByDescending(c => c.Revenue)
            .ToList();

        return BuildTopWithOthers(categoryRevenue, 5);
    }

    /// <summary>
    /// Calculates the top categories by quantity for the specified period.
    /// </summary>
    /// <param name="days">The number of days to look back.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of category breakdown responses ordered by quantity.</returns>
    private async Task<List<CategoryBreakdownResponse>> CalculateTopCategoriesByQuantityAsync(
        int days,
        CancellationToken cancellationToken = default)
    {
        var endDate = DateTime.UtcNow;
        var startDate = endDate.AddDays(-days);
        var orders = await orderService.GetAnalyticsOrdersAsync(startDate, endDate, cancellationToken);
        var orderList = orders.ToList();

        var categoryQuantity = orderList
            .SelectMany(o => o.Details)
            .GroupBy(d => d.Product.Category.Name)
            .Select(g => new CategoryBreakdownResponse
            {
                Category = g.Key,
                Revenue = g.Sum(d => d.Price * (decimal)d.Quantity),
                Quantity = g.Sum(d => d.Quantity),
                IsOther = false
            })
            .OrderByDescending(c => c.Quantity)
            .ToList();

        return BuildTopWithOthers(categoryQuantity, 5);
    }

    /// <summary>
    /// Calculates the daily sales summary.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The daily sales summary response.</returns>
    private async Task<DailySalesSummaryResponse> CalculateDailySalesSummaryAsync(
        CancellationToken cancellationToken = default)
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

    /// <summary>
    /// Calculates the accounts receivable for the specified period.
    /// </summary>
    /// <param name="days">The number of days to look back.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The accounts receivable response.</returns>
    private async Task<AccountsReceivableResponse> CalculateAccountsReceivableAsync(
        int days,
        CancellationToken cancellationToken = default)
    {
        var endDate = DateTime.UtcNow;
        var startDate = endDate.AddDays(-days);
        var orders = await orderService.GetAnalyticsOrdersAsync(startDate, endDate, cancellationToken);
        var orderList = orders.ToList();

        var accountsReceivable = new AccountsReceivableResponse
        {
            TotalPending = orderList.Where(o => o.Status == EnumOrderStatus.Pending).Sum(o => o.TotalAmount),
            PendingOrdersCount = orderList.Count(o => o.Status == EnumOrderStatus.Pending),
            TotalApproved = orderList.Where(o => o.Status == EnumOrderStatus.Approved).Sum(o => o.TotalAmount),
            ApprovedOrdersCount = orderList.Count(o => o.Status == EnumOrderStatus.Approved)
        };

        return accountsReceivable;
    }

    /// <summary>
    /// Calculates the profit margin trend for the specified period.
    /// </summary>
    /// <param name="days">The number of days to look back.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of profit margin trend responses.</returns>
    private async Task<List<ProfitMarginTrendResponse>> CalculateProfitMarginTrendAsync(
        int days,
        CancellationToken cancellationToken = default)
    {
        var endDate = DateTime.UtcNow;
        var startDate = endDate.AddDays(-days);
        var orders = await orderService.GetAnalyticsOrdersAsync(startDate, endDate, cancellationToken);
        var orderList = orders.ToList();

        var weeks = orderList
            .Select(o => o.SaleDate.Date)
            .Distinct()
            .Select(date => date.AddDays(-(int)date.DayOfWeek))
            .Distinct()
            .OrderBy(w => w)
            .ToList();

        var response = new List<ProfitMarginTrendResponse>();

        foreach (var week in weeks)
        {
            var weekNumber = GetWeekNumber(week);
            var weekOrders = orderList.Where(o => GetWeekNumber(o.SaleDate) == weekNumber).ToList();

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

    /// <summary>
    /// Calculates the revenue vs cost comparison for the specified period.
    /// </summary>
    /// <param name="days">The number of days to look back.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of revenue vs cost responses.</returns>
    private async Task<List<RevenueVsCostResponse>> CalculateRevenueVsCostAsync(
        int days,
        CancellationToken cancellationToken = default)
    {
        var endDate = DateTime.UtcNow;
        var startDate = endDate.AddDays(-days);
        var orders = await orderService.GetAnalyticsOrdersAsync(startDate, endDate, cancellationToken);
        var orderList = orders.ToList();

        var dailyData = orderList
            .GroupBy(o => o.SaleDate.Date)
            .OrderBy(g => g.Key)
            .Select(g => new
            {
                Date = g.Key,
                Revenue = g.Sum(o => o.TotalAmount),
                Cost = g.Sum(o => o.Details.Sum(d => d.Price * (decimal)d.Quantity * 0.7m)),
                Profit = g.Sum(o => o.TotalAmount) - g.Sum(o => o.Details.Sum(d => d.Price * (decimal)d.Quantity * 0.7m))
            })
            .ToList();

        const int bucketCount = 6;
        var bucketSize = dailyData.Count / bucketCount;
        var remainder = dailyData.Count % bucketCount;

        var response = new List<RevenueVsCostResponse>();
        var index = 0;

        for (var i = 0; i < bucketCount; i++)
        {
            var currentBucketSize = bucketSize + (i < remainder ? 1 : 0);
            if (currentBucketSize == 0)
            {
                break;
            }

            var bucket = dailyData.Skip(index).Take(currentBucketSize).ToList();
            index += currentBucketSize;

            var bucketRevenue = bucket.Sum(b => b.Revenue);
            var bucketCost = bucket.Sum(b => b.Cost);
            var bucketProfit = bucket.Sum(b => b.Profit);

            var start = bucket.First().Date;
            var end = bucket.Last().Date;
            var key = $"{start:dd/MM} - {end:dd/MM}";

            response.Add(new RevenueVsCostResponse(key, start, bucketRevenue, bucketCost, bucketProfit));
        }

        return response;
    }

    /// <summary>
    /// Calculates the KPI summary for the specified period.
    /// </summary>
    /// <param name="days">The number of days to look back.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The KPI summary response.</returns>
    private async Task<KpiSummaryResponse> CalculateKpiSummaryAsync(
        int days,
        CancellationToken cancellationToken = default)
    {
        var endDate = DateTime.UtcNow;
        var startDate = endDate.AddDays(-days);
        var orders = await orderService.GetAnalyticsOrdersAsync(startDate, endDate, cancellationToken);
        var orderList = orders.ToList();

        var totalRevenue = orderList.Sum(o => o.TotalAmount);
        var totalCost = orderList.Sum(o => o.Details.Sum(d => d.Price * (decimal)d.Quantity * 0.7m));
        var grossProfit = totalRevenue - totalCost;
        var profitMargin = totalRevenue > 0 ? grossProfit / totalRevenue * 100 : 0;
        var totalOrders = orderList.Count;
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