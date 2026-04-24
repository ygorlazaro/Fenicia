using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Enums.Auth;
using Fenicia.Module.Basic.Domains.Order.Queries;
using Fenicia.Module.Basic.Domains.Order.Responses;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Order.Handlers;

/// <summary>
///     Handler responsible for generating order analytics.
///     Provides comprehensive statistics including sales trends, top customers, and order values.
/// </summary>
public class GetOrderAnalyticsHandler(DefaultContext db)
{
    /// <summary>
    ///     Generates comprehensive order analytics for a given time period.
    /// </summary>
    /// <param name="query">Query containing days to analyze and top customers limit.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Complete analytics response with multiple data sets.</returns>
    public async Task<OrderAnalyticsResponse> Handle(GetOrderAnalyticsQuery query, CancellationToken ct)
    {
        var startDate = DateTime.UtcNow.AddDays(-query.Days);
        var endDate = DateTime.UtcNow;

        var orders = db.BasicOrders.Include(o => o.Customer).ThenInclude(c => c.Person).Include(o => o.Details).Where(o => o.SaleDate >= startDate && o.SaleDate <= endDate);

        var ordersByStatus = await GetOrdersByStatusAsync(orders, ct);
        var salesTrend = await GetSalesTrendAsync(orders, ct);
        var topCustomers = await GetTopCustomerAsync(query, orders, ct);
        var averageOrderValue = await GetAverageOrderValueAsync(orders, ct);
        var cancelledOrders = await GetCancelledOrderAsync(orders, ct);

        return new OrderAnalyticsResponse
        {
            OrdersByStatus = ordersByStatus,
            SalesTrend = salesTrend,
            TopCustomers = topCustomers,
            AverageOrderValue = averageOrderValue,
            CancelledOrders = cancelledOrders
        };
    }

    /// <summary>
    ///     Retrieves recent cancelled orders.
    /// </summary>
    private async Task<List<CancelledOrderResponse>> GetCancelledOrderAsync(IQueryable<OrderModel> orders, CancellationToken ct)
    {
        var cancelled = await orders
            .Where(o => o.Status == OrderStatus.Cancelled)
            .Select(o => new { o.Id, CustomerName = o.Customer.Person.Name, o.TotalAmount, o.SaleDate })
            .ToListAsync(ct);

        var orderIds = cancelled.Select(o => o.Id).ToList();

        var detailQtys = await db.BasicOrderDetails
            .Where(d => orderIds.Contains(d.OrderId))
            .GroupBy(d => d.OrderId)
            .Select(g => new { OrderId = g.Key, Qty = g.Sum(d => d.Quantity) })
            .ToDictionaryAsync(k => k.OrderId, v => v.Qty, ct);

        return cancelled
            .Select(o => new CancelledOrderResponse(
                o.Id,
                o.CustomerName,
                o.TotalAmount,
                o.SaleDate,
                (int)(detailQtys.TryGetValue(o.Id, out var q) ? q : 0),
                null))
            .OrderByDescending(o => o.SaleDate)
            .Take(20)
            .ToList();
    }

    /// <summary>
    ///     Calculates average order value statistics.
    /// </summary>
    private async Task<AverageOrderValueResponse> GetAverageOrderValueAsync(IQueryable<OrderModel> orders, CancellationToken ct)
    {
        var orderValues = await orders.Select(o => o.TotalAmount).OrderBy(v => v).ToListAsync(ct);
        var averageOrderValue = new AverageOrderValueResponse
        {
            TotalOrders = orderValues.Count,
            AverageValue = orderValues.Count > 0 ? orderValues.Average() : 0,
            MedianValue = orderValues.Count > 0 ? CalculateMedian(orderValues) : 0,
            MinValue = orderValues.Count > 0 ? orderValues.Min() : 0,
            MaxValue = orderValues.Count > 0 ? orderValues.Max() : 0
        };
        return averageOrderValue;
    }

    /// <summary>
    ///     Retrieves top customers by total spending.
    /// </summary>
    private async Task<List<TopCustomerResponse>> GetTopCustomerAsync(GetOrderAnalyticsQuery query, IQueryable<OrderModel> orders, CancellationToken ct)
    {
        var raw = await orders
            .Select(o => new { o.CustomerId, CustomerName = o.Customer.Person.Name, o.TotalAmount, o.Id })
            .ToListAsync(ct);

        var orderIds = raw.Select(o => o.Id).ToList();

        var detailQtys = await db.BasicOrderDetails
            .Where(d => orderIds.Contains(d.OrderId))
            .GroupBy(d => d.OrderId)
            .Select(g => new { OrderId = g.Key, Qty = g.Sum(d => d.Quantity) })
            .ToDictionaryAsync(k => k.OrderId, v => v.Qty, ct);

        var topCustomers = raw
            .GroupBy(o => new { o.CustomerId, o.CustomerName })
            .Select(g => new TopCustomerResponse(
                g.Key.CustomerId,
                g.Key.CustomerName,
                g.Count(),
                g.Sum(o => o.TotalAmount),
                g.Sum(o => (int)(detailQtys.TryGetValue(o.Id, out var q) ? q : 0))))
            .OrderByDescending(c => c.TotalSpent)
            .Take(query.TopCustomersLimit)
            .ToList();

        return topCustomers;
    }

    /// <summary>
    ///     Retrieves daily sales trends.
    /// </summary>
    private async Task<List<SalesTrendResponse>> GetSalesTrendAsync(IQueryable<OrderModel> orders, CancellationToken ct)
    {
        var orderData = await orders
            .Select(o => new { Date = o.SaleDate.Date, o.TotalAmount, o.Id })
            .ToListAsync(ct);

        var orderIds = orderData.Select(o => o.Id).ToList();

        var detailQtys = await db.BasicOrderDetails
            .Where(d => orderIds.Contains(d.OrderId))
            .GroupBy(d => d.OrderId)
            .Select(g => new { OrderId = g.Key, Qty = g.Sum(d => d.Quantity) })
            .ToDictionaryAsync(k => k.OrderId, v => v.Qty, ct);

        var salesTrend = orderData
            .GroupBy(o => o.Date)
            .Select(g => new SalesTrendResponse(
                g.Key.ToString("yyyy-MM-dd"),
                g.Key,
                g.Count(),
                g.Sum(o => o.TotalAmount),
                g.Sum(o => (int)(detailQtys.TryGetValue(o.Id, out var q) ? q : 0))))
            .OrderBy(s => s.Date)
            .ToList();

        return salesTrend;
    }

    /// <summary>
    ///     Groups orders by status and calculates counts and totals.
    /// </summary>
    private async Task<List<OrderStatusCountResponse>> GetOrdersByStatusAsync(IQueryable<OrderModel> orders, CancellationToken ct)
    {
        var groups = await orders
            .GroupBy(o => o.Status)
            .Select(g => new { Status = g.Key, Count = g.Count(), Total = g.Sum(o => o.TotalAmount) })
            .ToListAsync(ct);

        return groups
            .Select(g => new OrderStatusCountResponse(g.Status.ToString(), g.Count, g.Total))
            .OrderByDescending(s => s.Count)
            .ToList();
    }

    private static decimal CalculateMedian(List<decimal> values)
    {
        var count = values.Count;
        if (count == 0)
        {
            return 0;
        }

        var mid = count / 2;
        return count % 2 == 0 ? (values[mid - 1] + values[mid]) / 2 : values[mid];
    }
}
