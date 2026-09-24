using Fenicia.Common.DTOs.Basic.Order;

namespace Fenicia.Module.Basic.Domains.OrderDetail.Interfaces;

/// <summary>
/// Service interface for order detail operations.
/// </summary>
public interface IOrderDetailService
{
    /// <summary>
    /// Gets order details by order ID.
    /// </summary>
    /// <param name="orderId">The order ID.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of order detail responses.</returns>
    Task<List<OrderDetailResponse>> GetByOrderIdAsync(
        Guid orderId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets detail counts by order IDs.
    /// </summary>
    /// <param name="orderIds">The order IDs.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A dictionary mapping order IDs to detail counts.</returns>
    Task<Dictionary<Guid, int>> GetDetailCountsByOrderIdsAsync(
        IEnumerable<Guid> orderIds,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets quantity sums by order IDs.
    /// </summary>
    /// <param name="orderIds">The order IDs.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A dictionary mapping order IDs to quantity sums.</returns>
    Task<Dictionary<Guid, double>> GetQuantitySumsByOrderIdsAsync(
        IEnumerable<Guid> orderIds,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets order details by order date range.
    /// </summary>
    /// <param name="startDate">The start date.</param>
    /// <param name="endDate">The end date.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of order detail responses.</returns>
    Task<List<OrderDetailResponse>> GetByOrderDateRangeAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets order details by date range.
    /// </summary>
    /// <param name="startDate">The start date.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of order detail responses.</returns>
    Task<List<OrderDetailResponse>> GetByDateRangeAsync(DateTime startDate, CancellationToken cancellationToken = default);
}