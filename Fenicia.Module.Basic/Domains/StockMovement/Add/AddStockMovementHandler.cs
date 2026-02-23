using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Enums.Basic;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.StockMovement.Add;

public class AddStockMovementHandler(BasicContext context)
{
    public async Task<StockMovementResponse> Handle(AddStockMovementCommand command, CancellationToken ct)
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
            if (command.Type == StockMovementType.In)
            {
                product.Quantity += command.Quantity;
            }
            else if (command.Type == StockMovementType.Out)
            {
                product.Quantity -= command.Quantity;
            }

            context.Products.Update(product);
        }

        await context.SaveChangesAsync(ct);

        return new StockMovementResponse(stockMovement.Id, stockMovement.ProductId, stockMovement.Quantity, stockMovement.Date, stockMovement.Price, stockMovement.Type, stockMovement.CustomerId, stockMovement.SupplierId);
    }
}
