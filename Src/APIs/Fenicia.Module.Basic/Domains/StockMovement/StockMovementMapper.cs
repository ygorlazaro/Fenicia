using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.StockMovement.DTOs;
using Riok.Mapperly.Abstractions;

namespace Fenicia.Module.Basic.Domains.StockMovement;

[Mapper]
public static partial class StockMovementMapper
{
    public static GetStockMovementResponse MapToGetStockMovementResponse(this StockMovementModel movement)
    {
        return new GetStockMovementResponse(
            movement.Id,
            movement.ProductId,
            movement.Product.Name,
            movement.Quantity,
            movement.Date,
            movement.Price,
            movement.Type,
            movement.CustomerId,
            movement.Customer != null && movement.Customer.Person != null ? movement.Customer.Person.Name : null,
            movement.SupplierId,
            movement.Supplier != null && movement.Supplier.Person != null ? movement.Supplier.Person.Name : null,
            movement.EmployeeId,
            movement.Employee != null && movement.Employee.Person != null ? movement.Employee.Person.Name : null,
            movement.OrderId,
            movement.Reason);
    }

    public static AddStockMovementResponse MapToAddStockMovementResponse(this StockMovementModel movement)
    {
        return new AddStockMovementResponse(
            movement.Id,
            movement.ProductId,
            movement.Quantity,
            movement.Date,
            movement.Price ?? 0,
            movement.Type,
            movement.CustomerId,
            movement.SupplierId,
            movement.EmployeeId,
            movement.OrderId,
            movement.Reason);
    }

    public static UpdateStockMovementResponse MapToUpdateStockMovementResponse(this StockMovementModel movement)
    {
        return new UpdateStockMovementResponse(
            movement.Id,
            movement.ProductId,
            movement.Quantity,
            movement.Date,
            movement.Price ?? 0,
            movement.Type,
            movement.CustomerId,
            movement.SupplierId,
            movement.EmployeeId,
            movement.OrderId,
            movement.Reason);
    }
}
