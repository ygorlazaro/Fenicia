using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Enums.Basic;

namespace Fenicia.Module.Basic.Domains.Order.CreateOrder;

public class CreateOrderHandler(BasicContext context)
{
    public async Task<OrderResponse> Handle(CreateOrderCommand command, CancellationToken ct)
    {
        var details = command.Details.Select(d => new OrderDetailModel
        {
            Id = Guid.NewGuid(),
            ProductId = d.ProductId,
            Price = d.Price,
            Quantity = d.Quantity
        }).ToList();

        var order = new OrderModel
        {
            Id = Guid.NewGuid(),
            UserId = command.UserId,
            CustomerId = command.CustomerId,
            SaleDate = command.SaleDate,
            Status = command.Status,
            Details = details,
            TotalAmount = details.Select(d => d.Price * (decimal)d.Quantity).Sum()
        };

        context.Orders.Add(order);

        var stockMovements = details.Select(d => new StockMovementModel
        {
            Id = Guid.NewGuid(),
            Date = DateTime.Now,
            ProductId = d.ProductId,
            Type = StockMovementType.In,
            CustomerId = order.CustomerId,
            Quantity = d.Quantity,
            Price = d.Price
        });

        foreach (var stockMovement in stockMovements)
        {
            context.StockMovements.Add(stockMovement);
        }

        await context.SaveChangesAsync(ct);

        return new OrderResponse(order.Id, order.UserId, order.CustomerId, order.TotalAmount, order.SaleDate, order.Status);
    }
}
