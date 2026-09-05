using Fenicia.Common;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Enums.Basic;
using Fenicia.Module.Basic.Domains.Order.DTOs;
using Fenicia.Module.Basic.Domains.Order.Interfaces;
using Fenicia.Module.Basic.Domains.OrderDetail.Interfaces;
using Fenicia.Module.Basic.Domains.StockMovement.DTOs;
using Fenicia.Module.Basic.Domains.StockMovement.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Order;

public sealed class OrderService(
    IOrderRepository orderRepository,
    IOrderDetailService orderDetailService,
    IStockMovementService stockMovementService) : IOrderService
{
    public OrderService()
        : this(null!, null!, null!)
    {
    }

    public async Task<Pagination<List<GetAllOrderResponse>>> GetAllAsync(
        GetAllOrderQuery query,
        CancellationToken cancellationToken = default)
    {
        var baseQuery = orderRepository.Query()
            .Include(o => o.Customer).ThenInclude(c => c.Person);

        var filteredQuery = baseQuery;

        var total = await filteredQuery.CountAsync(cancellationToken);

        var orderIds = await filteredQuery
            .OrderByDescending(o => o.SaleDate)
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .Select(o => o.Id)
            .ToListAsync(cancellationToken);

        var detailCounts = await orderDetailService.GetDetailCountsByOrderIdsAsync(orderIds, cancellationToken);

        var orders = await (from o in orderRepository.Query()
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
            }).ToListAsync(cancellationToken);

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

    public async Task<GetOrderByIdResponse?> GetByIdAsync(
        GetOrderByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var order = await orderRepository.GetByIdWithDetailsAsync(query.Id, cancellationToken);

        return order?.MapToGetOrderByIdResponse();
    }

    public Task<List<Fenicia.Module.Basic.Domains.OrderDetail.DTOs.GetOrderDetailsByOrderIdResponse>>
        GetDetailsByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return orderDetailService.GetByOrderIdAsync(
            new Fenicia.Module.Basic.Domains.OrderDetail.DTOs.GetOrderDetailsByOrderIdQuery(orderId),
            cancellationToken);
    }

    public async Task<CreateOrderResponse> CreateAsync(
        CreateOrderCommand command,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        var orderId = Guid.NewGuid();

        var details = command.Details.Select(d =>
        {
            var subtotal = (d.Price * (decimal)d.Quantity) - d.DiscountAmount;
            return new OrderDetailModel
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                ProductId = d.ProductId,
                Price = d.Price,
                Quantity = d.Quantity,
                DiscountAmount = d.DiscountAmount,
                Subtotal = subtotal,
                CompanyId = companyId
            };
        }).ToList();

        var totalQuantity = details.Sum(d => (int)d.Quantity);
        var totalAmount = details.Sum(d => d.Subtotal);
        var orderNumber = GenerateOrderNumber();

        var order = new OrderModel
        {
            Id = orderId,
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
            EmployeeId = command.EmployeeId,
            CompanyId = companyId
        };

        var created = await orderRepository.InsertAsync(order, cancellationToken);

        foreach (var movementCommand in details.Select(detail => new StockMovementModel
                 {
                     Id = Guid.NewGuid(),
                     Date = DateTime.UtcNow,
                     ProductId = detail.ProductId,
                     Type = StockMovementType.Out,
                     CustomerId = created.CustomerId,
                     EmployeeId = created.EmployeeId,
                     OrderId = created.Id,
                     Quantity = detail.Quantity,
                     Price = detail.Price,
                     Reason = $"Sale order {created.Id}"
                 }).Select(stockMovement => new AddStockMovementCommand(
                     stockMovement.Id,
                     stockMovement.Quantity,
                     stockMovement.Date,
                     stockMovement.Price,
                     stockMovement.Type,
                     stockMovement.ProductId,
                     stockMovement.CustomerId,
                     stockMovement.SupplierId,
                     stockMovement.EmployeeId,
                     stockMovement.OrderId,
                     stockMovement.Reason)))
        {
            await stockMovementService.AddAsync(movementCommand, companyId, cancellationToken);
        }

        return created.MapToCreateOrderResponse();
    }

    public async Task DeleteAsync(
        DeleteOrderCommand command,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        await orderRepository.DeleteAsync(command.Id, cancellationToken);
    }

    public async Task<OrderAnalyticsResponse> GetAnalyticsAsync(
        GetOrderAnalyticsQuery query,
        CancellationToken cancellationToken = default)
    {
        var startDate = DateTime.UtcNow.AddDays(-query.Days);
        var endDate = DateTime.UtcNow;

        var response = await orderRepository.GetAnalyticsOrdersAsync(startDate, endDate, cancellationToken);
        var orders = response.ToList();

        var ordersByStatus = GetOrdersByStatus(orders);
        var salesTrend = await GetSalesTrendAsync(orders, cancellationToken);
        var topCustomers = await GetTopCustomerAsync(query, orders, cancellationToken);
        var averageOrderValue = GetAverageOrderValue(orders);
        var cancelledOrders = await GetCancelledOrderAsync(orders, cancellationToken);

        return new OrderAnalyticsResponse
        {
            OrdersByStatus = ordersByStatus,
            SalesTrend = salesTrend,
            TopCustomers = topCustomers,
            AverageOrderValue = averageOrderValue,
            CancelledOrders = cancelledOrders
        };
    }

    public Task<decimal> GetTotalRevenueAsync(CancellationToken cancellationToken = default)
    {
        return orderRepository.GetTotalRevenueAsync(cancellationToken);
    }

    public Task<decimal> GetTotalCostAsync(CancellationToken cancellationToken = default)
    {
        return orderRepository.GetTotalCostAsync(cancellationToken);
    }

    public Task<int> GetTotalOrdersCountAsync(CancellationToken cancellationToken = default)
    {
        return orderRepository.GetTotalOrdersCountAsync(cancellationToken);
    }

    public Task<List<DateTime>> GetOrderDatesAsync(CancellationToken cancellationToken = default)
    {
        return orderRepository.GetOrderDatesAsync(cancellationToken);
    }

    public Task<List<DateTime>> GetOrderWeeksAsync(CancellationToken cancellationToken = default)
    {
        return orderRepository.GetOrderWeeksAsync(cancellationToken);
    }

    public Task<decimal> GetTodayRevenueAsync(CancellationToken cancellationToken = default)
    {
        return orderRepository.GetTodayRevenueAsync(cancellationToken);
    }

    public Task<int> GetTodayOrdersCountAsync(CancellationToken cancellationToken = default)
    {
        return orderRepository.GetTodayOrdersCountAsync(cancellationToken);
    }

    public Task<decimal> GetWeekRevenueAsync(CancellationToken cancellationToken = default)
    {
        return orderRepository.GetWeekRevenueAsync(cancellationToken);
    }

    public Task<int> GetWeekOrdersCountAsync(CancellationToken cancellationToken = default)
    {
        return orderRepository.GetWeekOrdersCountAsync(cancellationToken);
    }

    public Task<decimal> GetMonthRevenueAsync(CancellationToken cancellationToken = default)
    {
        return orderRepository.GetMonthRevenueAsync(cancellationToken);
    }

    public Task<int> GetMonthOrdersCountAsync(CancellationToken cancellationToken = default)
    {
        return orderRepository.GetMonthOrdersCountAsync(cancellationToken);
    }

    public Task<decimal> GetLastMonthRevenueAsync(CancellationToken cancellationToken = default)
    {
        return orderRepository.GetLastMonthRevenueAsync(cancellationToken);
    }

    public Task<decimal> GetPendingAmountAsync(CancellationToken cancellationToken = default)
    {
        return orderRepository.GetPendingAmountAsync(cancellationToken);
    }

    public Task<int> GetPendingOrdersCountAsync(CancellationToken cancellationToken = default)
    {
        return orderRepository.GetPendingOrdersCountAsync(cancellationToken);
    }

    public Task<decimal> GetApprovedAmountAsync(CancellationToken cancellationToken = default)
    {
        return orderRepository.GetApprovedAmountAsync(cancellationToken);
    }

    public Task<int> GetApprovedOrdersCountAsync(CancellationToken cancellationToken = default)
    {
        return orderRepository.GetApprovedOrdersCountAsync(cancellationToken);
    }

    public Task<List<OrderModel>> GetRecentOrdersAsync(
        int topLimit,
        CancellationToken cancellationToken = default)
    {
        return orderRepository.GetRecentOrdersAsync(topLimit, cancellationToken);
    }

    public Task<List<OrderModel>> GetTopCustomerOrdersAsync(CancellationToken cancellationToken = default)
    {
        return orderRepository.GetTopCustomerOrdersAsync(cancellationToken);
    }

    public Task<List<OrderModel>> GetAtRiskOrdersAsync(CancellationToken cancellationToken = default)
    {
        return orderRepository.GetAtRiskOrdersAsync(cancellationToken);
    }

    public Task<List<OrderModel>> GetEmployeePerformanceOrdersAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        return orderRepository.GetEmployeePerformanceOrdersAsync(startDate, endDate, cancellationToken);
    }

    private static List<OrderStatusCountResponse> GetOrdersByStatus(IEnumerable<OrderModel> orders)
    {
        var groups = orders
            .GroupBy(o => o.Status)
            .Select(g => new { Status = g.Key, Count = g.Count(), Total = g.Sum(o => o.TotalAmount) })
            .ToList();

        return
        [
            .. groups
                .Select(g => new OrderStatusCountResponse(g.Status.ToString(), g.Count, g.Total))
                .OrderByDescending(s => s.Count)
        ];
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

    private static AverageOrderValueResponse GetAverageOrderValue(IEnumerable<OrderModel> orders)
    {
        var orderValues = orders.Select(o => o.TotalAmount).OrderBy(v => v).ToList();
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

    private async Task<List<CancelledOrderResponse>> GetCancelledOrderAsync(
        IEnumerable<OrderModel> orders,
        CancellationToken cancellationToken = default)
    {
        var cancelled = orders
            .Where(o => o.Status == OrderStatus.Cancelled)
            .Select(o => new { o.Id, CustomerName = o.Customer.Person.Name, o.TotalAmount, o.SaleDate })
            .ToList();

        var orderIds = cancelled.Select(o => o.Id).ToList();

        var detailQtys = await orderDetailService.GetQuantitySumsByOrderIdsAsync(orderIds, cancellationToken);

        return
        [
            .. cancelled
                .Select(o => new CancelledOrderResponse(
                    o.Id,
                    o.CustomerName,
                    o.TotalAmount,
                    o.SaleDate,
                    (int)(detailQtys.TryGetValue(o.Id, out var q) ? q : 0),
                    null))
                .OrderByDescending(o => o.SaleDate)
                .Take(20)
        ];
    }

    private async Task<List<TopCustomerResponse>> GetTopCustomerAsync(
        GetOrderAnalyticsQuery query,
        IEnumerable<OrderModel> orders,
        CancellationToken cancellationToken = default)
    {
        var raw = orders
            .Select(o => new { o.CustomerId, CustomerName = o.Customer.Person.Name, o.TotalAmount, o.Id })
            .ToList();

        var orderIds = raw.Select(o => o.Id).ToList();

        var detailQtys = await orderDetailService.GetQuantitySumsByOrderIdsAsync(orderIds, cancellationToken);

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

    private async Task<List<SalesTrendResponse>> GetSalesTrendAsync(
        IEnumerable<OrderModel> orders,
        CancellationToken cancellationToken = default)
    {
        var orderData = orders
            .Select(o => new { o.SaleDate.Date, o.TotalAmount, o.Id })
            .ToList();

        var orderIds = orderData.Select(o => o.Id).ToList();

        var detailQtys = await orderDetailService.GetQuantitySumsByOrderIdsAsync(orderIds, cancellationToken);

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
}