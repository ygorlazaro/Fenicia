using Fenicia.Common.Data.Mappers.Basic;
using Fenicia.Common.Data.Responses.Basic;

namespace Fenicia.Module.Basic.Domains.OrderDetail;

public class OrderDetailService(IOrderDetailRepository orderDetailRepository) : IOrderDetailService
{
    public async Task<List<OrderDetailResponse>> GetByOrderIdAsync(Guid orderId, CancellationToken ct)
    {
        var orderDetails = await orderDetailRepository.GetByOrderIdAsync(orderId, ct);

        return OrderDetailConverter.Map(orderDetails);
    }
}