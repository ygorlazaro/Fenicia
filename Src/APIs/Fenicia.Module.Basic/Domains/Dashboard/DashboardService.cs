using System.Globalization;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.DTOs.Basic.Dashboard;
using Fenicia.Common.Enums.Auth;
using Fenicia.Module.Basic.Domains.Dashboard.Interfaces;
using Fenicia.Module.Basic.Domains.Employee.Interfaces;
using Fenicia.Module.Basic.Domains.Order.Interfaces;
using Fenicia.Module.Basic.Domains.Product.Interfaces;

namespace Fenicia.Module.Basic.Domains.Dashboard;

public sealed class DashboardService(
    IOrderService orderService,
    IProductService productService,
    IEmployeeService employeeService) : IDashboardService
{
    public DashboardService()
        : this(null!, null!, null!)
    {
    }

    public async Task<FinancialDashboardResponse> GetFinancialDashboardAsync(
        GetFinancialDashboardQuery query,
        CancellationToken cancellationToken = default)
    {
        var kpi = await CalculateKpiSummaryAsync(query.Days, cancellationToken);
        var revenueVsCost = await CalculateRevenueVsCostAsync(query.Days, cancellationToken);
        var profitMarginTrend = await CalculateProfitMarginTrendAsync(query.Days, cancellationToken);
        var accountsReceivable = await CalculateAccountsReceivableAsync(query.Days, cancellationToken);
        var dailySales = await CalculateDailySalesSummaryAsync(cancellationToken);
        var topCategoriesByRevenue = await CalculateTopCategoriesByRevenueAsync(query.Days, cancellationToken);
        var topCategoriesByQuantity = await CalculateTopCategoriesByQuantityAsync(query.Days, cancellationToken);

        return DashboardMapper.MapToFinancialDashboardResponse(
            kpi,
            revenueVsCost,
            profitMarginTrend,
            accountsReceivable,
            dailySales,
            topCategoriesByRevenue,
            topCategoriesByQuantity);
    }

    public Task<decimal> GetTotalRevenueAsync(CancellationToken cancellationToken = default)
    {
        return orderService.GetTotalRevenueAsync(cancellationToken);
    }

    public Task<decimal> GetTotalCostAsync(CancellationToken cancellationToken = default)
    {
        return orderService.GetTotalCostAsync(cancellationToken);
    }

    public Task<int> GetTotalOrdersAsync(CancellationToken cancellationToken = default)
    {
        return orderService.GetTotalOrdersCountAsync(cancellationToken);
    }

    public Task<int> GetTotalProductsAsync(CancellationToken cancellationToken = default)
    {
        return productService.GetTotalProductsAsync(cancellationToken);
    }

    public Task<int> GetTotalEmployeesAsync(CancellationToken cancellationToken = default)
    {
        return employeeService.GetTotalEmployeesAsync(cancellationToken);
    }

    public Task<List<OrderModel>> GetRecentOrdersAsync(
        int topLimit,
        CancellationToken cancellationToken = default)
    {
        return orderService.GetRecentOrdersAsync(topLimit, cancellationToken);
    }

    public Task<List<OrderModel>> GetTopCustomerOrdersAsync(CancellationToken cancellationToken = default)
    {
        return orderService.GetTopCustomerOrdersAsync(cancellationToken);
    }

    public Task<List<OrderModel>> GetAtRiskOrdersAsync(CancellationToken cancellationToken = default)
    {
        return orderService.GetAtRiskOrdersAsync(cancellationToken);
    }

    public Task<List<OrderModel>> GetEmployeePerformanceOrdersAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        return orderService.GetEmployeePerformanceOrdersAsync(startDate, endDate, cancellationToken);
    }

    public Task<List<EmployeeModel>> GetAllEmployeesAsync(CancellationToken cancellationToken = default)
    {
        return employeeService.GetAllEmployeesAsync(cancellationToken);
    }

    private static int GetWeekNumber(DateTime date)
    {
        var culture = CultureInfo.CurrentCulture;
        var calendar = culture.Calendar;
        var weekRule = culture.DateTimeFormat.CalendarWeekRule;
        var firstDay = culture.DateTimeFormat.FirstDayOfWeek;
        return calendar.GetWeekOfYear(date, weekRule, firstDay);
    }

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
            TotalPending = orderList.Where(o => o.Status == OrderStatus.Pending).Sum(o => o.TotalAmount),
            PendingOrdersCount = orderList.Count(o => o.Status == OrderStatus.Pending),
            TotalApproved = orderList.Where(o => o.Status == OrderStatus.Approved).Sum(o => o.TotalAmount),
            ApprovedOrdersCount = orderList.Count(o => o.Status == OrderStatus.Approved)
        };

        return accountsReceivable;
    }

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