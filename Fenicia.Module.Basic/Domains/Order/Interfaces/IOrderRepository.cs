using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Repositories;

namespace Fenicia.Module.Basic.Domains.Order.Interfaces;

/// <summary>
/// Repository interface for order data access.
/// </summary>
public interface IOrderRepository : IRepository<OrderModel>
{
    /// <summary>
    /// Gets an order by ID with details.
    /// </summary>
    /// <param name="id">The order ID.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The order model if found, or null.</returns>
    Task<OrderModel?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets recent orders with pagination.
    /// </summary>
    /// <param name="page">The page number.</param>
    /// <param name="perPage">The number of items per page.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A collection of order models.</returns>
    Task<IEnumerable<OrderModel>> GetRecentOrdersAsync(
        int page,
        int perPage,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets recent order IDs with pagination.
    /// </summary>
    /// <param name="page">The page number.</param>
    /// <param name="perPage">The number of items per page.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of order IDs.</returns>
    Task<List<Guid>> GetRecentOrderIdsAsync(
        int page,
        int perPage,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets orders for analytics within a date range.
    /// </summary>
    /// <param name="startDate">The start date.</param>
    /// <param name="endDate">The end date.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A collection of order models.</returns>
    Task<IEnumerable<OrderModel>> GetAnalyticsOrdersAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total revenue within a date range.
    /// </summary>
    /// <param name="startDate">The start date.</param>
    /// <param name="endDate">The end date.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The total revenue.</returns>
    Task<decimal> GetTotalRevenueAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total cost within a date range.
    /// </summary>
    /// <param name="startDate">The start date.</param>
    /// <param name="endDate">The end date.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The total cost.</returns>
    Task<decimal> GetTotalCostAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total order count within a date range.
    /// </summary>
    /// <param name="startDate">The start date.</param>
    /// <param name="endDate">The end date.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The total order count.</returns>
    Task<int> GetTotalOrdersCountAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the order dates within a date range.
    /// </summary>
    /// <param name="startDate">The start date.</param>
    /// <param name="endDate">The end date.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of order dates.</returns>
    Task<List<DateTime>> GetOrderDatesAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total revenue.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The total revenue.</returns>
    Task<decimal> GetTotalRevenueAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total cost.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The total cost.</returns>
    Task<decimal> GetTotalCostAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total order count.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The total order count.</returns>
    Task<int> GetTotalOrdersCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the order dates.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of order dates.</returns>
    Task<List<DateTime>> GetOrderDatesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the order weeks.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of order week start dates.</returns>
    Task<List<DateTime>> GetOrderWeeksAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets today's revenue.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>Today's revenue.</returns>
    Task<decimal> GetTodayRevenueAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets today's order count.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>Today's order count.</returns>
    Task<int> GetTodayOrdersCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets this week's revenue.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>This week's revenue.</returns>
    Task<decimal> GetWeekRevenueAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets this week's order count.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>This week's order count.</returns>
    Task<int> GetWeekOrdersCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets this month's revenue.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>This month's revenue.</returns>
    Task<decimal> GetMonthRevenueAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets this month's order count.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>This month's order count.</returns>
    Task<int> GetMonthOrdersCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets last month's revenue.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>Last month's revenue.</returns>
    Task<decimal> GetLastMonthRevenueAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the pending amount.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The pending amount.</returns>
    Task<decimal> GetPendingAmountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the pending orders count.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The pending orders count.</returns>
    Task<int> GetPendingOrdersCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the approved amount.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The approved amount.</returns>
    Task<decimal> GetApprovedAmountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the approved orders count.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The approved orders count.</returns>
    Task<int> GetApprovedOrdersCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets recent orders up to a limit.
    /// </summary>
    /// <param name="topLimit">The maximum number of orders to return.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of order models.</returns>
    Task<List<OrderModel>> GetRecentOrdersAsync(int topLimit, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets top customer orders.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of order models.</returns>
    Task<List<OrderModel>> GetTopCustomerOrdersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets at-risk orders.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of order models.</returns>
    Task<List<OrderModel>> GetAtRiskOrdersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets employee performance orders within a date range.
    /// </summary>
    /// <param name="startDate">The start date.</param>
    /// <param name="endDate">The end date.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of order models.</returns>
    Task<List<OrderModel>> GetEmployeePerformanceOrdersAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);
}