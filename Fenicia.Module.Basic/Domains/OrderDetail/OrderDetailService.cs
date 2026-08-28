using Fenicia.Module.Basic.Domains.OrderDetail.DTOs;
using Fenicia.Module.Basic.Domains.OrderDetail;

namespace Fenicia.Module.Basic.Domains.OrderDetail;

public class OrderDetailService(OrderDetailRepository orderDetailRepository)
{
    public async Task<List<GetOrderDetailsByOrderIdResponse>> GetByOrderIdAsync(GetOrderDetailsByOrderIdQuery query, CancellationToken ct)
    {
        var details = await orderDetailRepository.GetByOrderIdAsync(query.OrderId, ct);

        return details.Select(d => new GetOrderDetailsByOrderIdResponse(
                d.Id,
                d.OrderId,
                d.ProductId,
                string.Empty,
                d.Price,
                d.DiscountAmount,
                d.Quantity,
                d.Subtotal))
            .ToList();
    }
}
