using Fenicia.Common;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Enums.Basic;
using Fenicia.Module.Basic.Domains.Order.DTOs;
using Fenicia.Module.Basic.Domains.OrderDetail;
using Fenicia.Module.Basic.Domains.StockMovement;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Order;

public class OrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly OrderDetailService _orderDetailService;
    private readonly StockMovementService _stockMovementService;

    public OrderService()
        : this(null!, null!, null!)
    {
    }

    public OrderService(
        IOrderRepository orderRepository,
        OrderDetailService orderDetailService,
        StockMovementService stockMovementService)
    {
        _orderRepository = orderRepository;
        _orderDetailService = orderDetailService;
        _stockMovementService = stockMovementService;
    }

    public virtual async Task<Pagination<List<GetAllOrderResponse>>> GetAllAsync(GetAllOrderQuery query, CancellationToken ct)
    {
        var total = await _orderRepository.CountAsync(ct);

        var orderIds = await _orderRepository.GetRecentOrderIdsAsync(query.Page, query.PerPage, ct);

        var detailCounts = await _orderDetailService.GetDetailCountsByOrderIdsAsync(orderIds, ct);

        var orders = await (from o in _orderRepository.Query()
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

    public virtual async Task<GetOrderByIdResponse?> GetByIdAsync(GetOrderByIdQuery query, CancellationToken ct)
    {
        var order = await _orderRepository.GetByIdWithDetailsAsync(query.Id, ct);

        if (order is null)
        {
            return null;
        }

        return order.MapToGetOrderByIdResponse();
    }

    public virtual async Task<CreateOrderResponse> CreateAsync(CreateOrderCommand command, Guid companyId, CancellationToken ct)
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
            EmployeeId = command.EmployeeId,
            CompanyId = companyId
        };

        var created = await _orderRepository.InsertAsync(order, ct);

        foreach (var detail in details)
        {
            var stockMovement = new StockMovementModel
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
            };

            var movementCommand = new Fenicia.Module.Basic.Domains.StockMovement.DTOs.AddStockMovementCommand(
                stockMovement.Id,
                stockMovement.Quantity,
                stockMovement.Date,
                stockMovement.Price ?? 0,
                stockMovement.Type,
                stockMovement.ProductId,
                stockMovement.CustomerId,
                stockMovement.SupplierId,
                stockMovement.EmployeeId,
                stockMovement.OrderId,
                stockMovement.Reason);

            await _stockMovementService.AddAsync(movementCommand, companyId, ct);
        }

        return created.MapToCreateOrderResponse();
    }

    public virtual async Task DeleteAsync(DeleteOrderCommand command, Guid companyId, CancellationToken ct)
    {
        await _orderRepository.DeleteAsync(command.Id, ct);
    }

    public virtual async Task<OrderAnalyticsResponse> GetAnalyticsAsync(GetOrderAnalyticsQuery query, CancellationToken ct)
    {
        var startDate = DateTime.UtcNow.AddDays(-query.Days);
        var endDate = DateTime.UtcNow;

        var orders = await _orderRepository.GetAnalyticsOrdersAsync(startDate, endDate, ct);

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

    public virtual async Task<int> GetCountAsync(CancellationToken ct)
    {
        return await _orderRepository.CountAsync(ct);
    }

    public virtual async Task<decimal> GetTotalRevenueAsync(CancellationToken ct)
    {
        return await _orderRepository.GetTotalRevenueAsync(ct);
    }

    public virtual async Task<decimal> GetTotalCostAsync(CancellationToken ct)
    {
        return await _orderRepository.GetTotalCostAsync(ct);
    }

    public virtual async Task<int> GetTotalOrdersCountAsync(CancellationToken ct)
    {
        return await _orderRepository.GetTotalOrdersCountAsync(ct);
    }

    public virtual async Task<List<DateTime>> GetOrderDatesAsync(CancellationToken ct)
    {
        return await _orderRepository.GetOrderDatesAsync(ct);
    }

    public virtual async Task<List<DateTime>> GetOrderWeeksAsync(CancellationToken ct)
    {
        return await _orderRepository.GetOrderWeeksAsync(ct);
    }

    public virtual async Task<decimal> GetTodayRevenueAsync(CancellationToken ct)
    {
        return await _orderRepository.GetTodayRevenueAsync(ct);
    }

    public virtual async Task<int> GetTodayOrdersCountAsync(CancellationToken ct)
    {
        return await _orderRepository.GetTodayOrdersCountAsync(ct);
    }

    public virtual async Task<decimal> GetWeekRevenueAsync(CancellationToken ct)
    {
        return await _orderRepository.GetWeekRevenueAsync(ct);
    }

    public virtual async Task<int> GetWeekOrdersCountAsync(CancellationToken ct)
    {
        return await _orderRepository.GetWeekOrdersCountAsync(ct);
    }

    public virtual async Task<decimal> GetMonthRevenueAsync(CancellationToken ct)
    {
        return await _orderRepository.GetMonthRevenueAsync(ct);
    }

    public virtual async Task<int> GetMonthOrdersCountAsync(CancellationToken ct)
    {
        return await _orderRepository.GetMonthOrdersCountAsync(ct);
    }

    public virtual async Task<decimal> GetLastMonthRevenueAsync(CancellationToken ct)
    {
        return await _orderRepository.GetLastMonthRevenueAsync(ct);
    }

    public virtual async Task<decimal> GetPendingAmountAsync(CancellationToken ct)
    {
        return await _orderRepository.GetPendingAmountAsync(ct);
    }

    public virtual async Task<int> GetPendingOrdersCountAsync(CancellationToken ct)
    {
        return await _orderRepository.GetPendingOrdersCountAsync(ct);
    }

    public virtual async Task<decimal> GetApprovedAmountAsync(CancellationToken ct)
    {
        return await _orderRepository.GetApprovedAmountAsync(ct);
    }

    public virtual async Task<int> GetApprovedOrdersCountAsync(CancellationToken ct)
    {
        return await _orderRepository.GetApprovedOrdersCountAsync(ct);
    }

    public virtual async Task<List<OrderModel>> GetRecentOrdersAsync(int topLimit, CancellationToken ct)
    {
        return await _orderRepository.GetRecentOrdersAsync(topLimit, ct);
    }

    public virtual async Task<List<OrderModel>> GetTopCustomerOrdersAsync(CancellationToken ct)
    {
        return await _orderRepository.GetTopCustomerOrdersAsync(ct);
    }

    public virtual async Task<List<OrderModel>> GetAtRiskOrdersAsync(CancellationToken ct)
    {
        return await _orderRepository.GetAtRiskOrdersAsync(ct);
    }

    public virtual async Task<List<OrderModel>> GetEmployeePerformanceOrdersAsync(DateTime startDate, DateTime endDate, CancellationToken ct)
    {
        return await _orderRepository.GetEmployeePerformanceOrdersAsync(startDate, endDate, ct);
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

    private async Task<List<CancelledOrderResponse>> GetCancelledOrderAsync(IEnumerable<OrderModel> orders, CancellationToken ct)
    {
        var cancelled = orders
                .Where(o => o.Status == OrderStatus.Cancelled)
            .Select(o => new { o.Id, CustomerName = o.Customer.Person.Name, o.TotalAmount, o.SaleDate })
            .ToList();

        var orderIds = cancelled.Select(o => o.Id).ToList();

        var detailQtys = await _orderDetailService.GetQuantitySumsByOrderIdsAsync(orderIds, ct);

        return [.. cancelled
            .Select(o => new CancelledOrderResponse(
                o.Id,
                o.CustomerName,
                o.TotalAmount,
                o.SaleDate,
                (int)(detailQtys.TryGetValue(o.Id, out var q) ? q : 0),
                null))
            .OrderByDescending(o => o.SaleDate)
            .Take(20)];
    }

    private async Task<AverageOrderValueResponse> GetAverageOrderValueAsync(IEnumerable<OrderModel> orders, CancellationToken ct)
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

    private async Task<List<TopCustomerResponse>> GetTopCustomerAsync(GetOrderAnalyticsQuery query, IEnumerable<OrderModel> orders, CancellationToken ct)
    {
        var raw = orders
                .Select(o => new { o.CustomerId, CustomerName = o.Customer.Person.Name, o.TotalAmount, o.Id })
            .ToList();

        var orderIds = raw.Select(o => o.Id).ToList();

        var detailQtys = await _orderDetailService.GetQuantitySumsByOrderIdsAsync(orderIds, ct);

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

    private async Task<List<SalesTrendResponse>> GetSalesTrendAsync(IEnumerable<OrderModel> orders, CancellationToken ct)
    {
        var orderData = orders
                .Select(o => new { Date = o.SaleDate.Date, o.TotalAmount, o.Id })
            .ToList();

        var orderIds = orderData.Select(o => o.Id).ToList();

        var detailQtys = await _orderDetailService.GetQuantitySumsByOrderIdsAsync(orderIds, ct);

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

    private async Task<List<OrderStatusCountResponse>> GetOrdersByStatusAsync(IEnumerable<OrderModel> orders, CancellationToken ct)
    {
        var groups = orders
                .GroupBy(o => o.Status)
            .Select(g => new { Status = g.Key, Count = g.Count(), Total = g.Sum(o => o.TotalAmount) })
            .ToList();

        return [.. groups
            .Select(g => new OrderStatusCountResponse(g.Status.ToString(), g.Count, g.Total))
            .OrderByDescending(s => s.Count)];
    }
}
