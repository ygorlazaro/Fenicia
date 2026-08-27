namespace Fenicia.Module.Basic.Domains.Product.DTOs.Responses;

public record WorstSellingProductResponse(

    Guid ProductId,

    string ProductName,

    string CategoryName,

    double TotalQuantitySold,

    decimal TotalRevenue,

    int OrderCount,

    double CurrentStock,

    decimal CostValue);