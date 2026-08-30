using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Repositories;

namespace Fenicia.Module.Basic.Domains.Order;

public interface IOrderRepository : IRepository<OrderModel>
{
    IQueryable<OrderModel> Query();

    Task<OrderModel?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default);

    Task<IEnumerable<OrderModel>> GetRecentOrdersAsync(int page = 1, int perPage = 10, CancellationToken ct = default);

    Task<List<Guid>> GetRecentOrderIdsAsync(int page = 1, int perPage = 10, CancellationToken ct = default);

    Task<IEnumerable<OrderModel>> GetAnalyticsOrdersAsync(DateTime startDate, DateTime endDate, CancellationToken ct = default);

    Task<decimal> GetTotalRevenueAsync(CancellationToken ct = default);

    Task<decimal> GetTotalCostAsync(CancellationToken ct = default);

    Task<int> GetTotalOrdersCountAsync(CancellationToken ct = default);

    Task<List<DateTime>> GetOrderDatesAsync(CancellationToken ct = default);

    Task<List<DateTime>> GetOrderWeeksAsync(CancellationToken ct = default);

    Task<decimal> GetTodayRevenueAsync(CancellationToken ct = default);

    Task<int> GetTodayOrdersCountAsync(CancellationToken ct = default);

    Task<decimal> GetWeekRevenueAsync(CancellationToken ct = default);

    Task<int> GetWeekOrdersCountAsync(CancellationToken ct = default);

    Task<decimal> GetMonthRevenueAsync(CancellationToken ct = default);

    Task<int> GetMonthOrdersCountAsync(CancellationToken ct = default);

    Task<decimal> GetLastMonthRevenueAsync(CancellationToken ct = default);

    Task<decimal> GetPendingAmountAsync(CancellationToken ct = default);

    Task<int> GetPendingOrdersCountAsync(CancellationToken ct = default);

    Task<decimal> GetApprovedAmountAsync(CancellationToken ct = default);

    Task<int> GetApprovedOrdersCountAsync(CancellationToken ct = default);

    Task<List<OrderModel>> GetRecentOrdersAsync(int topLimit, CancellationToken ct = default);

    Task<List<OrderModel>> GetTopCustomerOrdersAsync(CancellationToken ct = default);

    Task<List<OrderModel>> GetAtRiskOrdersAsync(CancellationToken ct = default);

    Task<List<OrderModel>> GetEmployeePerformanceOrdersAsync(DateTime startDate, DateTime endDate, CancellationToken ct = default);
}