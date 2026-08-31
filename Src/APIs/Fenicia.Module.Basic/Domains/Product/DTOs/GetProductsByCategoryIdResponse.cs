using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Basic.Domains.Product.DTOs;

public record GetProductsByCategoryIdResponse(
    [Required] Guid Id,
    [Required][MaxLength(200)] string Name,
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
    [Required] Guid CategoryId,
    [Required][MaxLength(200)] string CategoryName,
    bool IsActive);
