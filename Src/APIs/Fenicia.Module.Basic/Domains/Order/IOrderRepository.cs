using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Repositories;

namespace Fenicia.Module.Basic.Domains.Order;

public interface IOrderRepository : IRepository<OrderModel>
{
    IQueryable<OrderModel> Query();

    Task<OrderModel?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct);

    Task<IEnumerable<OrderModel>> GetRecentOrdersAsync(int page = 1, int perPage = 10, CancellationToken ct);

    Task<List<Guid>> GetRecentOrderIdsAsync(int page = 1, int perPage = 10, CancellationToken ct);

    Task<IEnumerable<OrderModel>> GetAnalyticsOrdersAsync(DateTime startDate, DateTime endDate, CancellationToken ct);

    Task<decimal> GetTotalRevenueAsync(CancellationToken ct);

    Task<decimal> GetTotalCostAsync(CancellationToken ct);

    Task<int> GetTotalOrdersCountAsync(CancellationToken ct);

    Task<List<DateTime>> GetOrderDatesAsync(CancellationToken ct);

    Task<List<DateTime>> GetOrderWeeksAsync(CancellationToken ct);

    Task<decimal> GetTodayRevenueAsync(CancellationToken ct);

    Task<int> GetTodayOrdersCountAsync(CancellationToken ct);

    Task<decimal> GetWeekRevenueAsync(CancellationToken ct);

    Task<int> GetWeekOrdersCountAsync(CancellationToken ct);

    Task<decimal> GetMonthRevenueAsync(CancellationToken ct);

    Task<int> GetMonthOrdersCountAsync(CancellationToken ct);

    Task<decimal> GetLastMonthRevenueAsync(CancellationToken ct);

    Task<decimal> GetPendingAmountAsync(CancellationToken ct);

    Task<int> GetPendingOrdersCountAsync(CancellationToken ct);

    Task<decimal> GetApprovedAmountAsync(CancellationToken ct);

    Task<int> GetApprovedOrdersCountAsync(CancellationToken ct);

    Task<List<OrderModel>> GetRecentOrdersAsync(int topLimit, CancellationToken ct);

    Task<List<OrderModel>> GetTopCustomerOrdersAsync(CancellationToken ct);

    Task<List<OrderModel>> GetAtRiskOrdersAsync(CancellationToken ct);

    Task<List<OrderModel>> GetEmployeePerformanceOrdersAsync(DateTime startDate, DateTime endDate, CancellationToken ct);
}
