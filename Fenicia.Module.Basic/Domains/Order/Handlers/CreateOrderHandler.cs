using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Enums.Basic;
using Fenicia.Module.Basic.Domains.Order.DTOs.Commands;
using Fenicia.Module.Basic.Domains.Order.DTOs.Responses;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Order.Handlers;

public class CreateOrderHandler(DefaultContext db) : IRequestHandler<CreateOrderCommand, CreateOrderResponse>
{
    public async Task<CreateOrderResponse> Handle(CreateOrderCommand command, CancellationToken ct)
    {
        var details = command.Details.Select(d =>
        {
            var subtotal = (d.Price * (decimal)d.Quantity) - d.DiscountAmount;
            return new OrderDetailModel
            {
                Id = Guid.NewGuid(),
                ProductId = d.ProductId,
                Price = d.Price,
                Quantity = d.Quantity,
                DiscountAmount = d.DiscountAmount,
                Subtotal = subtotal
            };
        }).ToList();

        var totalQuantity = details.Sum(d => (int)d.Quantity);
        var totalAmount = details.Sum(d => d.Subtotal);
        var orderNumber = GenerateOrderNumber();

        var order = new OrderModel
        {
            Id = Guid.NewGuid(),
            OrderNumber = orderNumber,
            UserId = command.UserId,
            CustomerId = command.CustomerId,
            SaleDate = command.SaleDate,
            Status = command.Status,
            Details = details,
            TotalAmount = totalAmount,
            DiscountAmount = command.DiscountAmount,
            TotalQuantity = totalQuantity,
            PaymentMethod = command.PaymentMethod,
            Notes = command.Notes,
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

            var product = await db.BasicProducts.FirstOrDefaultAsync(p => p.Id == detail.ProductId, ct);

            if (product is null)
            {
                continue;
            }

            product.Quantity -= detail.Quantity;
            db.Entry(product).State = EntityState.Modified;
        }

        await db.SaveChangesAsync(ct);

        return new CreateOrderResponse(
            order.Id,
            order.OrderNumber,
            order.UserId,
            order.CustomerId,
            order.TotalAmount,
            order.DiscountAmount,
            order.TotalQuantity,
            order.SaleDate,
            order.Status,
            order.PaymentMethod,
            order.Notes,
            order.EmployeeId);
    }

    private static string GenerateOrderNumber()
    {
        return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
    }
}
