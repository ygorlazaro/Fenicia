using Fenicia.Common;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.DTOs.Basic.Order;
using Fenicia.Common.DTOs.Basic.StockMovement;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Enums.Basic;
using Fenicia.Module.Basic.Domains.Order.Interfaces;
using Fenicia.Module.Basic.Domains.OrderDetail.Interfaces;
using Fenicia.Module.Basic.Domains.StockMovement.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Order;

/// <summary>
/// Service implementation for managing orders.
/// </summary>
/// <param name="repository">The order repository.</param>
/// <param name="orderDetailService">The order detail service.</param>
/// <param name="stockMovementService">The stock movement service.</param>
public sealed class OrderService(
    IOrderRepository repository,
    IOrderDetailService orderDetailService,
    IStockMovementService stockMovementService) : IOrderService
{
    /// <inheritdoc />
    public async Task<Pagination<List<OrderResponse>>> GetAllAsync(
        OrderQuery query,
        CancellationToken cancellationToken = default)
    {
        var baseQuery = repository.Query()
            .Include(o => o.Customer).ThenInclude(c => c.Person);

        var total = await baseQuery.CountAsync(cancellationToken);

        var orderIds = await baseQuery
            .OrderByDescending(o => o.SaleDate)
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .Select(o => o.Id)
            .ToListAsync(cancellationToken);

        var detailCounts = await orderDetailService.GetDetailCountsByOrderIdsAsync(orderIds, cancellationToken);

        var orders = await (from o in repository.Query()
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
            .Select(o => OrderMapper.MapToListResponse(
                new Fenicia.Common.Data.Models.Basic.OrderModel
                {
                    Id = o.Id,
                    OrderNumber = o.OrderNumber,
                    UserId = o.UserId,
                    CustomerId = o.CustomerId,
                    Customer = new CustomerModel { Person = new PersonModel { Name = o.CustomerName } },
                    TotalAmount = o.TotalAmount,
                    DiscountAmount = o.DiscountAmount,
                    TotalQuantity = o.TotalQuantity,
                    SaleDate = o.SaleDate,
                    Status = o.Status,
                    PaymentMethod = o.PaymentMethod,
                    EmployeeId = o.EmployeeId,
                    Employee = o.EmployeeId.HasValue ? new EmployeeModel { Person = new PersonModel { Name = o.EmployeeName } } : null
                },
                detailCounts.TryGetValue(o.Id, out var count) ? count : 0))
            .ToList();

        return new Pagination<List<OrderResponse>>(response, total, query);
    }

    /// <inheritdoc />
    public async Task<OrderResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var order = await repository.GetByIdWithDetailsAsync(id, cancellationToken);

        if (order is null)
        {
            return null;
        }

        return OrderMapper.MapToResponse(order);
    }

    /// <inheritdoc />
    public Task<List<OrderDetailResponse>> GetDetailsByOrderIdAsync(
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        return orderDetailService.GetByOrderIdAsync(orderId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<OrderResponse> CreateAsync(
        OrderRequest command,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        var orderId = Guid.NewGuid();

        var details = command.Details.Select(d =>
        {
            var subtotal = d.Price * (decimal)d.Quantity - d.DiscountAmount;
            return new Fenicia.Common.Data.Models.Basic.OrderDetailModel
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

        var order = new Fenicia.Common.Data.Models.Basic.OrderModel
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

        var created = await repository.InsertAsync(order, cancellationToken);

        foreach (var movementCommand in details.Select(detail => new StockMovementModel
        {
            Id = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            ProductId = detail.ProductId,
            Type = EnumStockMovementType.Out,
            CustomerId = created.CustomerId,
            EmployeeId = created.EmployeeId,
            OrderId = created.Id,
            Quantity = detail.Quantity,
            Price = detail.Price,
            Reason = $"Sale order {created.Id}"
        }).Select(stockMovement => new AddStockMovementRequest(
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

        return OrderMapper.MapToResponse(created);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(
        Guid id,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        await repository.DeleteAsync(id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<OrderAnalyticsResponse> GetAnalyticsAsync(
        OrderAnalyticsRequest request,
        CancellationToken cancellationToken = default)
    {
        var startDate = DateTime.UtcNow.AddDays(-request.Days);
        var endDate = DateTime.UtcNow;

        var response = await repository.GetAnalyticsOrdersAsync(startDate, endDate, cancellationToken);
        var orders = response.ToList();

        var ordersByStatus = GetOrdersByStatus(orders);
        var salesTrend = await GetSalesTrendAsync(orders, cancellationToken);
        var topCustomers = await GetTopCustomerAsync(request, orders, cancellationToken);
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

    /// <inheritdoc />
    public async Task<IEnumerable<Fenicia.Common.Data.Models.Basic.OrderModel>> GetAnalyticsOrdersAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        return await repository.GetAnalyticsOrdersAsync(startDate, endDate, cancellationToken);
    }

    /// <inheritdoc />
    public Task<decimal> GetTotalRevenueAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        return repository.GetTotalRevenueAsync(startDate, endDate, cancellationToken);
    }

    /// <inheritdoc />
    public Task<decimal> GetTotalCostAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        return repository.GetTotalCostAsync(startDate, endDate, cancellationToken);
    }

    /// <inheritdoc />
    public Task<int> GetTotalOrdersCountAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        return repository.GetTotalOrdersCountAsync(startDate, endDate, cancellationToken);
    }

    /// <inheritdoc />
    public Task<List<DateTime>> GetOrderDatesAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        return repository.GetOrderDatesAsync(startDate, endDate, cancellationToken);
    }

    /// <inheritdoc />
    public Task<decimal> GetTotalRevenueAsync(CancellationToken cancellationToken = default)
    {
        return repository.GetTotalRevenueAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<decimal> GetTotalCostAsync(CancellationToken cancellationToken = default)
    {
        return repository.GetTotalCostAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<int> GetTotalOrdersCountAsync(CancellationToken cancellationToken = default)
    {
        return repository.GetTotalOrdersCountAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<List<DateTime>> GetOrderDatesAsync(CancellationToken cancellationToken = default)
    {
        return repository.GetOrderDatesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<List<DateTime>> GetOrderWeeksAsync(CancellationToken cancellationToken = default)
    {
        return repository.GetOrderWeeksAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<decimal> GetTodayRevenueAsync(CancellationToken cancellationToken = default)
    {
        return repository.GetTodayRevenueAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<int> GetTodayOrdersCountAsync(CancellationToken cancellationToken = default)
    {
        return repository.GetTodayOrdersCountAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<decimal> GetWeekRevenueAsync(CancellationToken cancellationToken = default)
    {
        return repository.GetWeekRevenueAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<int> GetWeekOrdersCountAsync(CancellationToken cancellationToken = default)
    {
        return repository.GetWeekOrdersCountAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<decimal> GetMonthRevenueAsync(CancellationToken cancellationToken = default)
    {
        return repository.GetMonthRevenueAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<int> GetMonthOrdersCountAsync(CancellationToken cancellationToken = default)
    {
        return repository.GetMonthOrdersCountAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<decimal> GetLastMonthRevenueAsync(CancellationToken cancellationToken = default)
    {
        return repository.GetLastMonthRevenueAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<decimal> GetPendingAmountAsync(CancellationToken cancellationToken = default)
    {
        return repository.GetPendingAmountAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<int> GetPendingOrdersCountAsync(CancellationToken cancellationToken = default)
    {
        return repository.GetPendingOrdersCountAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<decimal> GetApprovedAmountAsync(CancellationToken cancellationToken = default)
    {
        return repository.GetApprovedAmountAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<int> GetApprovedOrdersCountAsync(CancellationToken cancellationToken = default)
    {
        return repository.GetApprovedOrdersCountAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<OrderResponse>> GetRecentOrdersAsync(
        int topLimit,
        CancellationToken cancellationToken = default)
    {
        var orders = await repository.GetRecentOrdersAsync(topLimit, cancellationToken);
        return [.. orders.Select(o => OrderMapper.MapToResponse(o))];
    }

    /// <inheritdoc />
    public async Task<List<OrderResponse>> GetTopCustomerOrdersAsync(CancellationToken cancellationToken = default)
    {
        var orders = await repository.GetTopCustomerOrdersAsync(cancellationToken);
        return [.. orders.Select(o => OrderMapper.MapToResponse(o))];
    }

    /// <inheritdoc />
    public async Task<List<OrderResponse>> GetAtRiskOrdersAsync(CancellationToken cancellationToken = default)
    {
        var orders = await repository.GetAtRiskOrdersAsync(cancellationToken);
        return [.. orders.Select(o => OrderMapper.MapToResponse(o))];
    }

    /// <inheritdoc />
    public async Task<List<OrderResponse>> GetEmployeePerformanceOrdersAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        var orders = await repository.GetEmployeePerformanceOrdersAsync(startDate, endDate, cancellationToken);
        return [.. orders.Select(o => OrderMapper.MapToResponse(o))];
    }

    /// <summary>
    /// Gets orders grouped by status.
    /// </summary>
    /// <param name="orders">The orders.</param>
    /// <returns>A list of order status count responses.</returns>
    private static List<OrderStatusCountResponse> GetOrdersByStatus(IEnumerable<Fenicia.Common.Data.Models.Basic.OrderModel> orders)
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

    /// <summary>
    /// Calculates the median of a list of decimal values.
    /// </summary>
    /// <param name="values">The values.</param>
    /// <returns>The median value.</returns>
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

    /// <summary>
    /// Generates a unique order number.
    /// </summary>
    /// <returns>The generated order number.</returns>
    private static string GenerateOrderNumber()
    {
        return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
    }

    /// <summary>
    /// Gets the average order value metrics.
    /// </summary>
    /// <param name="orders">The orders.</param>
    /// <returns>The average order value response.</returns>
    private static AverageOrderValueResponse GetAverageOrderValue(IEnumerable<Fenicia.Common.Data.Models.Basic.OrderModel> orders)
    {
        var orderValues = orders.Select(o => o.TotalAmount).OrderBy(v => v).ToList();
        return new AverageOrderValueResponse(
            orderValues.Count > 0 ? orderValues.Average() : 0,
            orderValues.Count,
            orderValues.Count > 0 ? CalculateMedian(orderValues) : 0,
            orderValues.Count > 0 ? orderValues.Min() : 0,
            orderValues.Count > 0 ? orderValues.Max() : 0);
    }

    /// <summary>
    /// Gets cancelled orders.
    /// </summary>
    /// <param name="orders">The orders.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of cancelled order responses.</returns>
    private async Task<List<CancelledOrderResponse>> GetCancelledOrderAsync(
        IEnumerable<Fenicia.Common.Data.Models.Basic.OrderModel> orders,
        CancellationToken cancellationToken = default)
    {
        var cancelled = orders
            .Where(o => o.Status == EnumOrderStatus.Cancelled)
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

    /// <summary>
    /// Gets top customers.
    /// </summary>
    /// <param name="request">The analytics request.</param>
    /// <param name="orders">The orders.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of top customer responses.</returns>
    private async Task<List<TopCustomerResponse>> GetTopCustomerAsync(
        OrderAnalyticsRequest request,
        IEnumerable<Fenicia.Common.Data.Models.Basic.OrderModel> orders,
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
            .Take(request.TopCustomersLimit)
            .ToList();

        return topCustomers;
    }

    /// <summary>
    /// Gets the sales trend.
    /// </summary>
    /// <param name="orders">The orders.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of sales trend responses.</returns>
    private async Task<List<SalesTrendResponse>> GetSalesTrendAsync(
        IEnumerable<Fenicia.Common.Data.Models.Basic.OrderModel> orders,
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
