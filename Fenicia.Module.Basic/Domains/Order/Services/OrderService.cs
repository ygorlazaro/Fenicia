using Fenicia.Common;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Enums.Basic;
using Fenicia.Module.Basic.Domains.Order.DTOs.Commands;
using Fenicia.Module.Basic.Domains.Order.DTOs.Queries;
using Fenicia.Module.Basic.Domains.Order.DTOs.Responses;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Order;

public class OrderService(DefaultContext db)
{
    public async Task<Pagination<List<GetAllOrderResponse>>> GetAllAsync(GetAllOrderQuery query, CancellationToken ct)
    {
        var total = await db.BasicOrders.CountAsync(ct);

        var orderIds = await db.BasicOrders
            .OrderByDescending(o => o.SaleDate)
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .Select(o => o.Id)
            .ToListAsync(ct);

        var detailCounts = await db.BasicOrderDetails
            .Where(d => orderIds.Contains(d.OrderId))
            .GroupBy(d => d.OrderId)
            .Select(g => new { OrderId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.OrderId, g => g.Count, ct);

        var orders = await (from o in db.BasicOrders
                            where orderIds.Contains(o.Id)
                            select new
                            {
                                o.Id,
                                o.OrderNumber,
                                o.UserId,
                                o.CustomerId,
                                CustomerName = o.Customer.Person.Name,
                                o.TotalAmount,
                                o.DiscountAmount,
                                o.TotalQuantity,
                                o.SaleDate,
                                o.Status,
                                o.PaymentMethod,
                                o.EmployeeId,
                                EmployeeName = o.Employee != null ? o.Employee.Person.Name : null
                            }).ToListAsync(ct);

        var response = orders
            .OrderByDescending(o => o.SaleDate)
            .Select(o => new GetAllOrderResponse(
                o.Id,
                o.OrderNumber,
                o.UserId,
                o.CustomerId,
                o.CustomerName,
                o.TotalAmount,
                o.DiscountAmount,
                o.TotalQuantity,
                o.SaleDate,
                o.Status.ToString(),
                o.PaymentMethod,
                detailCounts.TryGetValue(o.Id, out var count) ? count : 0,
                o.EmployeeId,
                o.EmployeeName))
            .ToList();

        return new Pagination<List<GetAllOrderResponse>>(response, total, query.Page, query.PerPage);
    }

    public async Task<GetOrderByIdResponse?> GetByIdAsync(GetOrderByIdQuery query, CancellationToken ct)
    {
        var order = await db.BasicOrders
            .Include(o => o.Customer).ThenInclude(c => c.Person)
            .Include(o => o.Details).ThenInclude(d => d.Product)
            .FirstOrDefaultAsync(o => o.Id == query.Id, ct);

        if (order is null)
        {
            return null;
        }

        return new GetOrderByIdResponse(
            order.Id,
            order.OrderNumber,
            order.UserId,
            order.CustomerId,
            order.Customer?.Person?.Name ?? "Unknown",
            order.TotalAmount,
            order.DiscountAmount,
            order.TotalQuantity,
            order.SaleDate,
            order.Status.ToString(),
            order.PaymentMethod,
            order.Notes,
            order.EmployeeId);
    }

    public async Task<CreateOrderResponse> CreateAsync(CreateOrderCommand command, CancellationToken ct)
    {
        var details = command.Details.Select(d =>
        {
            var subtotal = (d.Price * (decimal)d.Quantity) - d.DiscountAmount;
            return new OrderDetailModel
            {
                Id = Guid.NewGuid(),
                ProductId = d.ProductId,
                Price = d.Price,
                Quantity = d.Quantity,
                DiscountAmount = d.DiscountAmount,
                Subtotal = subtotal
            };
        }).ToList();

        var totalQuantity = details.Sum(d => (int)d.Quantity);
        var totalAmount = details.Sum(d => d.Subtotal);
        var orderNumber = GenerateOrderNumber();

        var order = new OrderModel
        {
            Id = Guid.NewGuid(),
            OrderNumber = orderNumber,
            UserId = command.UserId,
            CustomerId = command.CustomerId,
            SaleDate = command.SaleDate,
            Status = command.Status,
            Details = details,
            TotalAmount = totalAmount,
            DiscountAmount = command.DiscountAmount,
            TotalQuantity = totalQuantity,
            PaymentMethod = command.PaymentMethod,
            Notes = command.Notes,
            EmployeeId = command.EmployeeId
        };

        db.BasicOrders.Add(order);

        foreach (var detail in details)
        {
            var stockMovement = new StockMovementModel
            {
                Id = Guid.NewGuid(),
                Date = DateTime.UtcNow,
                ProductId = detail.ProductId,
                Type = StockMovementType.Out,
                CustomerId = order.CustomerId,
                EmployeeId = order.EmployeeId,
                OrderId = order.Id,
                Quantity = detail.Quantity,
                Price = detail.Price,
                Reason = $"Sale order {order.Id}"
            };

            db.BasicStockMovements.Add(stockMovement);

            var product = await db.BasicProducts.FirstOrDefaultAsync(p => p.Id == detail.ProductId, ct);

            if (product is null)
            {
                continue;
            }

            product.Quantity -= detail.Quantity;
            db.Entry(product).State = EntityState.Modified;
        }

        await db.SaveChangesAsync(ct);

        return new CreateOrderResponse(
            order.Id,
            order.OrderNumber,
            order.UserId,
            order.CustomerId,
            order.TotalAmount,
            order.DiscountAmount,
            order.TotalQuantity,
            order.SaleDate,
            order.Status,
            order.PaymentMethod,
            order.Notes,
            order.EmployeeId);
    }

    public async Task DeleteAsync(DeleteOrderCommand command, CancellationToken ct)
    {
        var order = await db.BasicOrders.FirstOrDefaultAsync(o => o.Id == command.Id, ct);

        if (order is not null)
        {
            order.Deleted = DateTime.UtcNow;
            db.BasicOrders.Update(order);
            await db.SaveChangesAsync(ct);
        }
    }

    public async Task<OrderAnalyticsResponse> GetAnalyticsAsync(GetOrderAnalyticsQuery query, CancellationToken ct)
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

    private static string GenerateOrderNumber()
    {
        return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
    }
}
