namespace Fenicia.Module.Basic.Domains.Supplier.DTOs.Responses;

public record SupplierStockMovementResponse(

    Guid MovementId,

    Guid ProductId,

    string ProductName,

    double Quantity,

    decimal Price,

    DateTime Date,

    string MovementType);