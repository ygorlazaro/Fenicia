namespace Fenicia.Module.Basic.Domains.Inventory.Responses;

public record OverstockProductResponse(
    Guid ProductId,
    string ProductName,
    string CategoryName,
    double CurrentQuantity,
    double RecommendedQuantity,
    decimal ExcessValue,
    decimal CostPrice);
