using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Repositories;

namespace Fenicia.Module.Basic.Domains.Order;

public interface IOrderRepository : IRepository<OrderModel>
{
    Task<OrderModel?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IEnumerable<OrderModel>> GetRecentOrdersAsync(int page = 1, int perPage = 10, CancellationToken cancellationToken = default);

    Task<List<Guid>> GetRecentOrderIdsAsync(int page = 1, int perPage = 10, CancellationToken cancellationToken = default);

    Task<IEnumerable<OrderModel>> GetAnalyticsOrdersAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

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
