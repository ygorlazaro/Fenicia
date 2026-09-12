using System.ComponentModel.DataAnnotations;
using Fenicia.Common;

namespace Fenicia.Common.DTOs.Basic.Product;

public record GetAllProductResponse([Required] Guid Id,
    [Required] [MaxLength(200)] string Name,
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
    [Required] [MaxLength(200)] string CategoryName,
    Guid? SupplierId,
    string? SupplierName,
    bool IsActive) : ICrudItem;