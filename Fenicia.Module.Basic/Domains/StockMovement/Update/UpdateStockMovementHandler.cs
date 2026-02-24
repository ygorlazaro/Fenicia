using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.StockMovement.Update;

public class UpdateStockMovementHandler(BasicContext context)
{
    public async Task<UpdateStockMovementResponse?> Handle(UpdateStockMovementCommand command, CancellationToken ct)
    {
        var stockMovement = await context.StockMovements
            .Include(s => s.Product)
            .FirstOrDefaultAsync(s => s.Id == command.Id, ct);

        if (stockMovement is null)
        {
            return null;
        }

        stockMovement.Date = command.Date;
        stockMovement.Type = command.Type;
        stockMovement.ProductId = command.ProductId;
        stockMovement.CustomerId = command.CustomerId;
        stockMovement.Quantity = command.Quantity;
        stockMovement.Price = command.Price;
        stockMovement.SupplierId = command.SupplierId;

        context.StockMovements.Update(stockMovement);

        await context.SaveChangesAsync(ct);

        return new UpdateStockMovementResponse(stockMovement.Id, stockMovement.ProductId, stockMovement.Quantity, stockMovement.Date, stockMovement.Price, stockMovement.Type, stockMovement.CustomerId, stockMovement.SupplierId);
    }
}