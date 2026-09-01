using Fenicia.Common;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Order.DTOs;

namespace Fenicia.Module.Basic.Domains.Order.Interfaces;

public interface IOrderService
{
    Task<Pagination<List<GetAllOrderResponse>>> GetAllAsync(GetAllOrderQuery query, CancellationToken cancellationToken = default);

    Task<GetOrderByIdResponse?> GetByIdAsync(GetOrderByIdQuery query, CancellationToken cancellationToken = default);

    Task<CreateOrderResponse> CreateAsync(CreateOrderCommand command, Guid companyId, CancellationToken cancellationToken = default);

    Task DeleteAsync(DeleteOrderCommand command, Guid companyId, CancellationToken cancellationToken = default);

    Task<OrderAnalyticsResponse> GetAnalyticsAsync(GetOrderAnalyticsQuery query, CancellationToken cancellationToken = default);

    Task<decimal> GetTotalRevenueAsync(CancellationToken cancellationToken = default);

    Task<decimal> GetTotalCostAsync(CancellationToken cancellationToken = default);

    Task<int> GetTotalOrdersCountAsync(CancellationToken cancellationToken = default);

    Task<List<DateTime>> GetOrderDatesAsync(CancellationToken cancellationToken = default);

    Task<List<DateTime>> GetOrderWeeksAsync(CancellationToken cancellationToken = default);

    Task<decimal> GetTodayRevenueAsync(CancellationToken cancellationToken = default);

    Task<int> GetTodayOrdersCountAsync(CancellationToken cancellationToken = default);

    Task<decimal> GetWeekRevenueAsync(CancellationToken cancellationToken = default);

    Task<int> GetWeekOrdersCountAsync(CancellationToken cancellationToken = default);

    Task<decimal> GetMonthRevenueAsync(CancellationToken cancellationToken = default);

    Task<int> GetMonthOrdersCountAsync(CancellationToken cancellationToken = default);

    Task<decimal> GetLastMonthRevenueAsync(CancellationToken cancellationToken = default);

    Task<decimal> GetPendingAmountAsync(CancellationToken cancellationToken = default);

    Task<int> GetPendingOrdersCountAsync(CancellationToken cancellationToken = default);

    Task<decimal> GetApprovedAmountAsync(CancellationToken cancellationToken = default);

    Task<int> GetApprovedOrdersCountAsync(CancellationToken cancellationToken = default);

    Task<List<OrderModel>> GetRecentOrdersAsync(int topLimit, CancellationToken cancellationToken = default);

    Task<List<OrderModel>> GetTopCustomerOrdersAsync(CancellationToken cancellationToken = default);

    Task<List<OrderModel>> GetAtRiskOrdersAsync(CancellationToken cancellationToken = default);

    Task<List<OrderModel>> GetEmployeePerformanceOrdersAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
}
