using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.OrderDetail.DTOs;
using Fenicia.Module.Basic.Domains.OrderDetail.Interfaces;

namespace Fenicia.Module.Basic.Domains.OrderDetail;

public sealed class OrderDetailService(IOrderDetailRepository orderDetailRepository) : IOrderDetailService
{
    public OrderDetailService()
        : this(null!)
    {
    }

    public async Task<List<GetOrderDetailsByOrderIdResponse>> GetByOrderIdAsync(
        GetOrderDetailsByOrderIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var details = await orderDetailRepository.GetByOrderIdAsync(query.OrderId, cancellationToken);

        return [.. details.Select(d => d.MapToGetOrderDetailsByOrderIdResponse())];
    }

    public Task<Dictionary<Guid, int>> GetDetailCountsByOrderIdsAsync(
        IEnumerable<Guid> orderIds,
        CancellationToken cancellationToken = default)
    {
        return orderDetailRepository.GetDetailCountsByOrderIdsAsync(orderIds, cancellationToken);
    }

    public Task<Dictionary<Guid, double>> GetQuantitySumsByOrderIdsAsync(
        IEnumerable<Guid> orderIds,
        CancellationToken cancellationToken = default)
    {
        return orderDetailRepository.GetQuantitySumsByOrderIdsAsync(orderIds, cancellationToken);
    }

    public async Task<List<OrderDetailModel>> GetByOrderDateRangeAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        var result = await orderDetailRepository.GetByOrderDateRangeAsync(startDate, endDate, cancellationToken);
        return [.. result];
    }

    public async Task<List<OrderDetailModel>> GetByDateRangeAsync(
        DateTime startDate,
        CancellationToken cancellationToken = default)
    {
        var result = await orderDetailRepository.GetByDateRangeAsync(startDate, cancellationToken);
        return [.. result];
    }
}