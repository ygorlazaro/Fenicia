using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.DTOs.Basic.StockMovement;

namespace Fenicia.Module.Basic.Domains.StockMovement;

/// <summary>
/// Mapper class for mapping between StockMovementModel and StockMovement DTOs.
/// </summary>
public static class StockMovementMapper
{
    /// <summary>
    /// Maps a StockMovementModel to a GetStockMovementResponse.
    /// </summary>
    /// <param name="movement">The stock movement model.</param>
    /// <returns>The stock movement response.</returns>
    public static GetStockMovementResponse MapToGetStockMovementResponse(StockMovementModel movement)
    {
        return new GetStockMovementResponse(
            movement.Id,
            movement.Date,
            movement.Type,
            movement.Quantity,
            movement.Price,
            movement.ProductId,
            movement.CustomerId,
            movement.SupplierId,
            movement.EmployeeId,
            movement.OrderId,
            movement.Reason,
            movement.Product?.Name ?? string.Empty,
            movement.Product?.Category?.Name ?? string.Empty,
            movement.Customer?.Person?.Name ?? string.Empty,
            movement.Supplier?.Person?.Name ?? string.Empty,
            movement.Employee?.Person?.Name ?? string.Empty);
    }

    /// <summary>
    /// Maps an AddStockMovementRequest to a StockMovementModel.
    /// </summary>
    /// <param name="request">The add stock movement request.</param>
    /// <param name="companyId">The company identifier.</param>
    /// <returns>The stock movement model.</returns>
    public static StockMovementModel MapToStockMovementModel(AddStockMovementRequest request, Guid companyId)
    {
        return new StockMovementModel
        {
            Id = request.Id,
            Date = request.Date,
            Type = request.Type,
            Quantity = request.Quantity,
            Price = request.Price,
            ProductId = request.ProductId,
            CustomerId = request.CustomerId,
            SupplierId = request.SupplierId,
            EmployeeId = request.EmployeeId,
            OrderId = request.OrderId,
            Reason = request.Reason,
            CompanyId = companyId
        };
    }

    /// <summary>
    /// Maps an UpdateStockMovementRequest to a StockMovementModel.
    /// </summary>
    /// <param name="movement">The existing stock movement model.</param>
    /// <param name="request">The update stock movement request.</param>
    /// <returns>The updated stock movement model.</returns>
    public static StockMovementModel MapToUpdatedStockMovementModel(StockMovementModel movement, UpdateStockMovementRequest request)
    {
        movement.Date = request.Date;
        movement.Type = request.Type;
        movement.Quantity = request.Quantity;
        movement.Price = request.Price;
        movement.ProductId = request.ProductId;
        movement.CustomerId = request.CustomerId;
        movement.SupplierId = request.SupplierId;
        movement.EmployeeId = request.EmployeeId;
        movement.OrderId = request.OrderId;
        movement.Reason = request.Reason;
        return movement;
    }
}
