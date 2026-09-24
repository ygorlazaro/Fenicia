using Fenicia.Common;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.DTOs.Basic.Order;

namespace Fenicia.Module.Basic.Domains.Order.Interfaces;

/// <summary>
/// Service interface for order operations.
/// </summary>
public interface IOrderService
{
    /// <summary>
    /// Gets a paginated list of orders.
    /// </summary>
    /// <param name="query">The pagination and filter query.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A paginated list of order responses.</returns>
    Task<Pagination<List<OrderResponse>>> GetAllAsync(
        OrderQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an order by ID.
    /// </summary>
    /// <param name="id">The order ID.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The order response if found, or null.</returns>
    Task<OrderResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets order details by order ID.
    /// </summary>
    /// <param name="orderId">The order ID.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of order detail responses.</returns>
    Task<List<OrderDetailResponse>> GetDetailsByOrderIdAsync(
        Guid orderId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new order.
    /// </summary>
    /// <param name="command">The order request data.</param>
    /// <param name="companyId">The company ID.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The created order response.</returns>
    Task<OrderResponse> CreateAsync(
        OrderRequest command,
        Guid companyId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an order.
    /// </summary>
    /// <param name="id">The order ID.</param>
    /// <param name="companyId">The company ID.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteAsync(Guid id, Guid companyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets order analytics.
    /// </summary>
    /// <param name="request">The analytics request parameters.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The order analytics response.</returns>
    Task<OrderAnalyticsResponse> GetAnalyticsAsync(
        OrderAnalyticsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets orders for analytics within a date range.
    /// </summary>
    /// <param name="startDate">The start date.</param>
    /// <param name="endDate">The end date.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A collection of order responses.</returns>
    Task<IEnumerable<OrderResponse>> GetAnalyticsOrdersAsync(
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
    /// <returns>A list of order responses.</returns>
    Task<List<OrderResponse>> GetRecentOrdersAsync(int topLimit, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets top customer orders.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of order responses.</returns>
    Task<List<OrderResponse>> GetTopCustomerOrdersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets at-risk orders.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of order responses.</returns>
    Task<List<OrderResponse>> GetAtRiskOrdersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets employee performance orders within a date range.
    /// </summary>
    /// <param name="startDate">The start date.</param>
    /// <param name="endDate">The end date.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of order responses.</returns>
    Task<List<OrderResponse>> GetEmployeePerformanceOrdersAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);
}