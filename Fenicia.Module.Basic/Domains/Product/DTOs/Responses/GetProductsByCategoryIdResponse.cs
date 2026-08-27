namespace Fenicia.Module.Basic.Domains.Product.DTOs.Responses;

public record GetProductsByCategoryIdResponse(
    Guid Id,
    string Name,
    string? SKU,
    string? Barcode,
    string? Description,
    decimal? CostPrice,
    decimal SalesPrice,
    double Quantity,
    int? MinStockLevel,
    int? MaxStockLevel,
    string? ImageUrl,
    decimal? Weight,
    string? Dimensions,
    string? UnitOfMeasure,
    Guid CategoryId,
    string CategoryName,
    bool IsActive);