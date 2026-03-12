namespace Fenicia.Module.Basic.Domains.Product.Responses;

public record NeverSoldProductResponse(
    Guid ProductId,
    string ProductName,
    string CategoryName,
    string? SupplierName,
    double CurrentStock,
    decimal CostValue,
    DateTime? LastStockMovement);
