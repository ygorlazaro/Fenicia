using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.OrderDetail.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.OrderDetail;

public class OrderDetailService
{
    private readonly IOrderDetailRepository _orderDetailRepository;

    public OrderDetailService()
        : this(null!)
    {
    }

    public OrderDetailService(IOrderDetailRepository orderDetailRepository)
    {
        _orderDetailRepository = orderDetailRepository;
    }

    public virtual async Task<List<GetOrderDetailsByOrderIdResponse>> GetByOrderIdAsync(GetOrderDetailsByOrderIdQuery query, CancellationToken ct)
    {
        var details = await _orderDetailRepository.GetByOrderIdAsync(query.OrderId, ct);

        return [.. details.Select(d => d.MapToGetOrderDetailsByOrderIdResponse())];
    }

    public virtual async Task<Dictionary<Guid, int>> GetDetailCountsByOrderIdsAsync(IEnumerable<Guid> orderIds, CancellationToken ct)
    {
        return await _orderDetailRepository.GetDetailCountsByOrderIdsAsync(orderIds, ct);
    }

    public virtual async Task<Dictionary<Guid, double>> GetQuantitySumsByOrderIdsAsync(IEnumerable<Guid> orderIds, CancellationToken ct)
    {
        return await _orderDetailRepository.GetQuantitySumsByOrderIdsAsync(orderIds, ct);
    }

    public virtual async Task<List<OrderDetailModel>> GetByOrderDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken ct)
    {
        var result = await _orderDetailRepository.GetByOrderDateRangeAsync(startDate, endDate, ct);
        return [.. result];
    }

    public virtual async Task<List<OrderDetailModel>> GetByDateRangeAsync(DateTime startDate, CancellationToken ct)
    {
        var result = await _orderDetailRepository.GetByDateRangeAsync(startDate, ct);
        return [.. result];
    }
}
