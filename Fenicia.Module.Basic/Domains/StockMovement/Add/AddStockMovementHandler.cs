using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Enums.Basic;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.StockMovement.Add;

public class AddStockMovementHandler(BasicContext context)
{
    public async Task<AddStockMovementResponse> Handle(AddStockMovementCommand command, CancellationToken ct)
    {
        var stockMovement = new StockMovementModel
        {
            Id = command.Id,
            Quantity = command.Quantity,
            Date = command.Date,
            Price = command.Price,
            Type = command.Type,
            ProductId = command.ProductId,
            CustomerId = command.CustomerId,
            SupplierId = command.SupplierId
        };

        context.StockMovements.Add(stockMovement);

        var product = await context.Products.FirstOrDefaultAsync(p => p.Id == command.ProductId, ct);

        if (product is not null)
        {
            product.Quantity = command.Type switch
            {
                StockMovementType.In => product.Quantity += command.Quantity,
                StockMovementType.Out => product.Quantity -= command.Quantity,
                _ => throw new ArgumentOutOfRangeException()
            };

            context.Products.Update(product);
        }

        await context.SaveChangesAsync(ct);

        return new AddStockMovementResponse(stockMovement.Id, stockMovement.ProductId, stockMovement.Quantity, stockMovement.Date, stockMovement.Price, stockMovement.Type, stockMovement.CustomerId, stockMovement.SupplierId);
    }
}