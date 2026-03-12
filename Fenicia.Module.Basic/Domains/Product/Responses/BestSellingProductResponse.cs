namespace Fenicia.Module.Basic.Domains.Product.Responses;

public record BestSellingProductResponse(
    Guid ProductId,
    string ProductName,
    string CategoryName,
    double TotalQuantitySold,
    decimal TotalRevenue,
    int OrderCount,
    decimal AveragePrice);
