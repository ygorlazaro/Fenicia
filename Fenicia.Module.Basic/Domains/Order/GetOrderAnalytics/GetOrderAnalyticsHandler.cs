using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Enums.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Order.GetOrderAnalytics;

public class GetOrderAnalyticsHandler(DefaultContext context)
{
    public async Task<OrderAnalyticsResponse> Handle(GetOrderAnalyticsQuery query, CancellationToken ct)
    {
        var startDate = DateTime.UtcNow.AddDays(-query.Days);
        var endDate = DateTime.UtcNow;

        var orders = await context.BasicOrders
            .Include(o => o.CustomerModel)
            .ThenInclude(c => c.PersonModel)
            .Include(o => o.Details)
            .Where(o => o.SaleDate >= startDate && o.SaleDate <= endDate)
            .ToListAsync(ct);

        // 1. Orders by Status
        var ordersByStatus = orders
            .GroupBy(o => o.Status)
            .Select(g => new OrderStatusCountResponse(
                g.Key.ToString(),
                g.Count(),
                g.Sum(o => o.TotalAmount)))
            .OrderByDescending(s => s.Count)
            .ToList();

        // 2. Sales Trend (daily)
        var salesTrend = orders
            .GroupBy(o => new { o.SaleDate.Date })
            .Select(g => new SalesTrendResponse(
                g.Key.Date.ToString("yyyy-MM-dd"),
                g.Key.Date,
                g.Count(),
                g.Sum(o => o.TotalAmount),
                g.Sum(o => o.Details.Sum(d => (int)d.Quantity))))
            .OrderBy(s => s.Date)
            .ToList();

        // 3. Top Customers
        var topCustomers = orders
            .GroupBy(o => new { o.CustomerId, CustomerName = o.CustomerModel.PersonModel.Name })
            .Select(g => new TopCustomerResponse(
                g.Key.CustomerId,
                g.Key.CustomerName,
                g.Count(),
                g.Sum(o => o.TotalAmount),
                g.Sum(o => o.Details.Sum(d => (int)d.Quantity))))
            .OrderByDescending(c => c.TotalSpent)
            .Take(query.TopCustomersLimit)
            .ToList();

        // 4. Average Order Value
        var orderValues = orders.Select(o => o.TotalAmount).OrderBy(v => v).ToList();
        var averageOrderValue = new AverageOrderValueResponse
        {
            TotalOrders = orderValues.Count,
            AverageValue = orderValues.Count > 0 ? orderValues.Average() : 0,
            MedianValue = orderValues.Count > 0 ? CalculateMedian(orderValues) : 0,
            MinValue = orderValues.Count > 0 ? orderValues.Min() : 0,
            MaxValue = orderValues.Count > 0 ? orderValues.Max() : 0
        };

        // 5. Cancelled Orders
        var cancelledOrders = orders
            .Where(o => o.Status == OrderStatus.Cancelled)
            .Select(o => new CancelledOrderResponse(
                o.Id,
                o.CustomerModel.PersonModel.Name,
                o.TotalAmount,
                o.SaleDate,
                o.Details.Sum(d => (int)d.Quantity),
                null))
            .OrderByDescending(o => o.SaleDate)
            .Take(20)
            .ToList();

        return new OrderAnalyticsResponse
        {
            OrdersByStatus = ordersByStatus,
            SalesTrend = salesTrend,
            TopCustomers = topCustomers,
            AverageOrderValue = averageOrderValue,
            CancelledOrders = cancelledOrders
        };
    }

    private static decimal CalculateMedian(List<decimal> values)
    {
        var count = values.Count;
        if (count == 0) return 0;

        var mid = count / 2;
        return count % 2 == 0
            ? (values[mid - 1] + values[mid]) / 2
            : values[mid];
    }
}
