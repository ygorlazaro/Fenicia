using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Basic.Domains.Product.DTOs;

public record UpdateProductCommand(
    Guid Id,
    [Required] [MaxLength(50)] string Name,
    [MaxLength(50)] string? SKU = null,
    [MaxLength(50)] string? Barcode = null,
    [MaxLength(1000)] string? Description = null,
    decimal? CostPrice = null,
    [Required] decimal SalesPrice = 0,
    [Required] double Quantity = 0,
    int? MinStockLevel = null,
    int? MaxStockLevel = null,
    [MaxLength(500)] string? ImageUrl = null,
    decimal? Weight = null,
    [MaxLength(50)] string? Dimensions = null,
    [MaxLength(20)] string? UnitOfMeasure = null,
    [Required] Guid CategoryId = default,
    Guid? SupplierId = null);