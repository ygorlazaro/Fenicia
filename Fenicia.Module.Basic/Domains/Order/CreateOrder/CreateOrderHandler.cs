using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models;
using Fenicia.Common.Enums.Basic;

namespace Fenicia.Module.Basic.Domains.Order.CreateOrder;

public class CreateOrderHandler(DefaultContext context)
{
    public async Task<CreateOrderResponse> Handle(CreateOrderCommand command, CancellationToken ct)
    {
        var details = command.Details.Select(d => new BasicOrderDetailModel
        {
            Id = Guid.NewGuid(),
            ProductId = d.ProductId,
            Price = d.Price,
            Quantity = d.Quantity
        }).ToList();

        var order = new BasicOrderModel
        {
            Id = Guid.NewGuid(),
            UserId = command.UserId,
            CustomerId = command.CustomerId,
            SaleDate = command.SaleDate,
            Status = command.Status,
            Details = details,
            TotalAmount = details.Select(d => d.Price * (decimal)d.Quantity).Sum(),
            EmployeeId = command.EmployeeId
        };

        context.BasicOrders.Add(order);

        // Create stock movements for each product and update product quantity
        foreach (var detail in details)
        {
            // Create stock movement (Out = subtracting from stock)
            var stockMovement = new BasicStockMovementModel
            {
                Id = Guid.NewGuid(),
                Date = DateTime.UtcNow,
                ProductId = detail.ProductId,
                Type = StockMovementType.Out,
                CustomerId = order.CustomerId,
                EmployeeId = order.EmployeeId,
                OrderId = order.Id,
                Quantity = detail.Quantity,
                Price = detail.Price,
                Reason = $"Sale order {order.Id}"
            };

            context.BasicStockMovements.Add(stockMovement);

            // Update product quantity
            var product = await context.BasicProducts.FindAsync([detail.ProductId], ct);
            if (product is not null)
            {
                product.Quantity -= detail.Quantity;
                context.BasicProducts.Update(product);
            }
        }

        await context.SaveChangesAsync(ct);

        return new CreateOrderResponse(order.Id, order.UserId, order.CustomerId, order.TotalAmount, order.SaleDate, order.Status, order.EmployeeId);
    }
}
