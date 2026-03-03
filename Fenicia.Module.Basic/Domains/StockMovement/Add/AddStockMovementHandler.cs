using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models;
using Fenicia.Common.Enums.Basic;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.StockMovement.Add;

public class AddStockMovementHandler(DefaultContext context)
{
    public async Task<AddStockMovementResponse> Handle(AddStockMovementCommand command, CancellationToken ct)
    {
        var stockMovement = new BasicStockMovementModel
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

        context.BasicStockMovements.Add(stockMovement);

        var product = await context.BasicProducts.FirstOrDefaultAsync(p => p.Id == command.ProductId, ct);

        if (product is not null)
        {
            product.Quantity = command.Type switch
            {
                StockMovementType.In => product.Quantity += command.Quantity,
                StockMovementType.Out => product.Quantity -= command.Quantity,
                _ => throw new ArgumentOutOfRangeException()
            };

            context.BasicProducts.Update(product);
        }

        await context.SaveChangesAsync(ct);

        return new AddStockMovementResponse(stockMovement.Id, stockMovement.ProductId, stockMovement.Quantity, stockMovement.Date, stockMovement.Price, stockMovement.Type, stockMovement.CustomerId, stockMovement.SupplierId);
    }
}
