namespace Fenicia.Module.Basic.Domains.Product.Responses;

public record WorstSellingProductResponse(
    Guid ProductId,
    string ProductName,
    string CategoryName,
    double TotalQuantitySold,
    decimal TotalRevenue,
    int OrderCount,
    double CurrentStock,
    decimal CostValue);
