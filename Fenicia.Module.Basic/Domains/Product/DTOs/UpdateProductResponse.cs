namespace Fenicia.Module.Basic.Domains.Product.DTOs;

public record UpdateProductResponse(
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
    Guid? SupplierId,
    string? SupplierName,
    bool IsActive);
