using Fenicia.Common.Data.Converters.Basic;
using Fenicia.Common.Data.Responses.Basic;

namespace Fenicia.Module.Basic.Domains.OrderDetail;

public class OrderDetailService(IOrderDetailRepository orderDetailRepository) : IOrderDetailService
{
    public async Task<List<OrderDetailResponse>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var orderDetails = await orderDetailRepository.GetByOrderIdAsync(orderId, cancellationToken);

        return OrderDetailConverter.Convert(orderDetails);
    }
}
