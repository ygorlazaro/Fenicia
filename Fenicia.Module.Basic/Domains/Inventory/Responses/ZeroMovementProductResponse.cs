namespace Fenicia.Module.Basic.Domains.Inventory.Responses;

public record ZeroMovementProductResponse(
    Guid ProductId,
    string ProductName,
    string CategoryName,
    string? SupplierName,
    double CurrentStock,
    decimal StockValue,
    DateTime? LastMovementDate,
    int DaysWithoutMovement);
