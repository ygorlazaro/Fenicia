using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Enums.Basic;
using Fenicia.Common.Localization;
using Fenicia.Module.Basic.Domains.StockMovement.Commands;
using Fenicia.Module.Basic.Domains.StockMovement.Responses;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.StockMovement.Handlers;

/// <summary>
///     Handler responsible for creating a new stock movement.
///     Also updates the product quantity based on the movement type (In/Out).
/// </summary>
public class AddStockMovementHandler(DefaultContext db)
{
    /// <summary>
    ///     Creates a new stock movement and updates product quantity.
    /// </summary>
    /// <param name="command">The command containing stock movement details.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created stock movement with its details.</returns>
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
            SupplierId = command.SupplierId,
            EmployeeId = command.EmployeeId,
            OrderId = command.OrderId,
            Reason = command.Reason
        };

        db.BasicStockMovements.Add(stockMovement);

        var product = await db.BasicProducts.FirstOrDefaultAsync(p => p.Id == command.ProductId, ct);

        if (product is not null)
        {
            product.Quantity = command.Type switch
            {
                StockMovementType.In => product.Quantity += command.Quantity,
                StockMovementType.Out => product.Quantity -= command.Quantity,
                _ => throw new ArgumentOutOfRangeException(nameof(command.Type), ExceptionMessages.InvalidRequest)
            };

            db.BasicProducts.Update(product);
        }

        await db.SaveChangesAsync(ct);

        return new AddStockMovementResponse(stockMovement.Id, stockMovement.ProductId, stockMovement.Quantity, stockMovement.Date, stockMovement.Price, stockMovement.Type, stockMovement.CustomerId, stockMovement.SupplierId, stockMovement.EmployeeId, stockMovement.OrderId, stockMovement.Reason);
    }
}