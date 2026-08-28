namespace Fenicia.Module.Basic.Domains.Supplier.DTOs;

public record SupplierStockMovementResponse(

    Guid MovementId,

    Guid ProductId,

    string ProductName,

    double Quantity,

    decimal Price,

    DateTime Date,

    string MovementType);
