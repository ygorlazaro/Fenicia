using Fenicia.Common.Data.Converters.Basic;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Requests.Basic;
using Fenicia.Common.Data.Responses.Basic;
using Fenicia.Common.Enums;
using Fenicia.Module.Basic.Domains.OrderDetail;
using Fenicia.Module.Basic.Domains.StockMoviment;

namespace Fenicia.Module.Basic.Domains.Order;

public class OrderService(IOrderRepository orderRepository, IOrderDetailRepository orderDetailRepository, IStockMovementRepository stockMovementRepository) : IOrderService
{
    public async Task<OrderResponse?> AddAsync(OrderRequest orderRequest, CancellationToken cancellationToken)
    {
        var order = OrderConverter.Convert(orderRequest);
        var total = order.Details.Sum(d => d.Price);

        order.TotalAmount = total;

        orderRepository.Add(order);
        orderDetailRepository.AddRange(order.Details);

        var stockMovement = order.Details.Select(d => new StockMovementModel
        {
            Date = DateTime.Now,
            ProductId = d.ProductId,
            Type = StockMovementType.In,
            CustomerId = order.CustomerId,
            Quantity = d.Quantity,
            Price = d.Price
        });

        stockMovementRepository.AddRange(stockMovement);

        await orderRepository.SaveChangesAsync(cancellationToken);

        return OrderConverter.Convert(order);
    }
}
