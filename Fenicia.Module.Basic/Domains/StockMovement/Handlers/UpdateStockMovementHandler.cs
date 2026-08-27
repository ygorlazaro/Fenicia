using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.StockMovement.DTOs.Commands;
using Fenicia.Module.Basic.Domains.StockMovement.DTOs.Responses;
using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.StockMovement.Handlers;

public class UpdateStockMovementHandler(DefaultContext db) : IRequestHandler<UpdateStockMovementCommand, UpdateStockMovementResponse?>
{

    public async Task<UpdateStockMovementResponse?> Handle(UpdateStockMovementCommand command, CancellationToken ct)
    {
        var stockMovement = await db.BasicStockMovements.Include(s => s.Product).FirstOrDefaultAsync(s => s.Id == command.Id, ct);

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
        stockMovement.EmployeeId = command.EmployeeId;
        stockMovement.OrderId = command.OrderId;
        stockMovement.Reason = command.Reason;

        db.BasicStockMovements.Update(stockMovement);

        await db.SaveChangesAsync(ct);

        return new UpdateStockMovementResponse(stockMovement.Id, stockMovement.ProductId, stockMovement.Quantity, stockMovement.Date, stockMovement.Price, stockMovement.Type, stockMovement.CustomerId, stockMovement.SupplierId, stockMovement.EmployeeId, stockMovement.OrderId, stockMovement.Reason);
    }
}