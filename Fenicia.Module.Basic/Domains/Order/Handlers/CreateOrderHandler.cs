using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Enums.Basic;
using Fenicia.Module.Basic.Domains.Order.Commands;
using Fenicia.Module.Basic.Domains.Order.Responses;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Order.Handlers;

public class CreateOrderHandler(DefaultContext db)
{
    public async Task<CreateOrderResponse> Handle(CreateOrderCommand command, CancellationToken ct)
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
            TotalAmount = details.Select(d => d.Price * (decimal)d.Quantity).Sum(),
            EmployeeId = command.EmployeeId
        };

        db.BasicOrders.Add(order);

        foreach (var detail in details)
        {
            var stockMovement = new StockMovementModel
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

            db.BasicStockMovements.Add(stockMovement);

            var product = await db.BasicProducts.FirstOrDefaultAsync(p => p.Id == detail.ProductId,
                ct);

            if (product is null)
            {
                continue;
            }

            product.Quantity -= detail.Quantity;
            db.Entry(product).State = EntityState.Modified;
        }

        await db.SaveChangesAsync(ct);

        return new CreateOrderResponse(order.Id,
            order.UserId,
            order.CustomerId,
            order.TotalAmount,
            order.SaleDate,
            order.Status,
            order.EmployeeId);
    }
}
