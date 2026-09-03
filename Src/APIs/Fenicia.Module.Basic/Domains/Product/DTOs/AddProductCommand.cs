using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Basic.Domains.Product.DTOs;

public record AddProductCommand(
    [Required] Guid Id,
    [Required] [MaxLength(200)] string Name,
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
    [Required] Guid CategoryId = default,
    Guid? SupplierId = null);