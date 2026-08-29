using Fenicia.Module.Basic.Domains.OrderDetail.DTOs;

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
}
