using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.DTOs.Basic.Order;
using Fenicia.Module.Basic.Domains.OrderDetail.Interfaces;

namespace Fenicia.Module.Basic.Domains.OrderDetail;

/// <summary>
/// Service implementation for managing order details.
/// </summary>
/// <param name="repository">The order detail repository.</param>
public sealed class OrderDetailService(IOrderDetailRepository repository) : IOrderDetailService
{
    /// <inheritdoc />
    public async Task<List<OrderDetailResponse>> GetByOrderIdAsync(
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        var details = await repository.GetByOrderIdAsync(orderId, cancellationToken);
        return OrderDetailMapper.MapToResponseList(details);
    }

    /// <inheritdoc />
    public Task<Dictionary<Guid, int>> GetDetailCountsByOrderIdsAsync(
        IEnumerable<Guid> orderIds,
        CancellationToken cancellationToken = default)
    {
        return repository.GetDetailCountsByOrderIdsAsync(orderIds, cancellationToken);
    }

    /// <inheritdoc />
    public Task<Dictionary<Guid, double>> GetQuantitySumsByOrderIdsAsync(
        IEnumerable<Guid> orderIds,
        CancellationToken cancellationToken = default)
    {
        return repository.GetQuantitySumsByOrderIdsAsync(orderIds, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<OrderDetailResponse>> GetByOrderDateRangeAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        var result = await repository.GetByOrderDateRangeAsync(startDate, endDate, cancellationToken);
        return OrderDetailMapper.MapToResponseList(result);
    }

    /// <inheritdoc />
    public async Task<List<OrderDetailResponse>> GetByDateRangeAsync(
        DateTime startDate,
        CancellationToken cancellationToken = default)
    {
        var result = await repository.GetByDateRangeAsync(startDate, cancellationToken);
        return OrderDetailMapper.MapToResponseList(result);
    }
}