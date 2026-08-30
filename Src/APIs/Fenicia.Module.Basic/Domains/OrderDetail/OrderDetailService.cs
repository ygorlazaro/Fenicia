using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.OrderDetail.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.OrderDetail;

public class OrderDetailService(OrderDetailRepository orderDetailRepository)
{
    public OrderDetailService()
        : this(null!)
    {
    }

    public async Task<List<GetOrderDetailsByOrderIdResponse>> GetByOrderIdAsync(GetOrderDetailsByOrderIdQuery query, CancellationToken ct)
    {
        var details = await orderDetailRepository.GetByOrderIdAsync(query.OrderId, ct);

        return details.Select(d => d.MapToGetOrderDetailsByOrderIdResponse()).ToList();
    }

    public async Task<Dictionary<Guid, int>> GetDetailCountsByOrderIdsAsync(IEnumerable<Guid> orderIds, CancellationToken ct)
    {
        return await orderDetailRepository.GetDetailCountsByOrderIdsAsync(orderIds, ct);
    }

    public async Task<Dictionary<Guid, double>> GetQuantitySumsByOrderIdsAsync(IEnumerable<Guid> orderIds, CancellationToken ct)
    {
        return await orderDetailRepository.GetQuantitySumsByOrderIdsAsync(orderIds, ct);
    }

    public async Task<List<OrderDetailModel>> GetByOrderDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken ct = default)
    {
        var result = await orderDetailRepository.GetByOrderDateRangeAsync(startDate, endDate, ct);
        return result.ToList();
    }

    public async Task<List<OrderDetailModel>> GetByDateRangeAsync(DateTime startDate, CancellationToken ct = default)
    {
        var result = await orderDetailRepository.GetByDateRangeAsync(startDate, ct);
        return result.ToList();
    }
}
