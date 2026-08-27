namespace Fenicia.Module.Basic.Domains.Product.DTOs.Responses;

public record NeverSoldProductResponse(

    Guid ProductId,

    string ProductName,

    string CategoryName,

    string? SupplierName,

    double CurrentStock,

    decimal CostValue,

    DateTime? LastStockMovement);