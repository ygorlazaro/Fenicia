using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Repositories;

namespace Fenicia.Module.Basic.Domains.OrderDetail.Interfaces;

/// <summary>
/// Repository interface for managing order detail data.
/// </summary>
public interface IOrderDetailRepository : IRepository<OrderDetailModel>
{
    /// <summary>
    /// Gets order details by order ID.
    /// </summary>
    /// <param name="orderId">The order ID.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A collection of order detail models.</returns>
    Task<IEnumerable<OrderDetailModel>> GetByOrderIdAsync(
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
    /// <returns>A collection of order detail models.</returns>
    Task<IEnumerable<OrderDetailModel>> GetByOrderDateRangeAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets order details by date range.
    /// </summary>
    /// <param name="startDate">The start date.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A collection of order detail models.</returns>
    Task<IEnumerable<OrderDetailModel>> GetByDateRangeAsync(
        DateTime startDate,
        CancellationToken cancellationToken = default);
}