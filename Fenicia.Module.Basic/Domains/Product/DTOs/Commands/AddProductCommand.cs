using Fenicia.Module.Basic.Domains.Product.DTOs.Responses;

namespace Fenicia.Module.Basic.Domains.Product.DTOs.Commands;

public record AddProductCommand(
    Guid Id,
    string Name,
    string? SKU = null,
    string? Barcode = null,
    string? Description = null,
    decimal? CostPrice = null,
    decimal SalesPrice = 0,
    double Quantity = 0,
    int? MinStockLevel = null,
    int? MaxStockLevel = null,
    string? ImageUrl = null,
    decimal? Weight = null,
    string? Dimensions = null,
    string? UnitOfMeasure = null,
    Guid CategoryId = default,
    Guid? SupplierId = null);