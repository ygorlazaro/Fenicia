using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Product;

/// <summary>
/// Response DTO containing product data.
/// </summary>
/// <param name="id">The unique identifier of the product.</param>
/// <param name="name">The product name.</param>
/// <param name="sku">The product SKU.</param>
/// <param name="barcode">The product barcode.</param>
/// <param name="description">The product description.</param>
/// <param name="costPrice">The cost price.</param>
/// <param name="salesPrice">The sales price.</param>
/// <param name="quantity">The available quantity.</param>
/// <param name="minStockLevel">The minimum stock level.</param>
/// <param name="maxStockLevel">The maximum stock level.</param>
/// <param name="imageUrl">The product image URL.</param>
/// <param name="weight">The product weight.</param>
/// <param name="dimensions">The product dimensions.</param>
/// <param name="unitOfMeasure">The unit of measure.</param>
/// <param name="categoryId">The category ID.</param>
/// <param name="categoryName">The category name.</param>
/// <param name="supplierId">The supplier ID.</param>
/// <param name="supplierName">The supplier name.</param>
/// <param name="isActive">Whether the product is active.</param>
public record ProductResponse(
    [Required] Guid Id,
    [Required] [MaxLength(50)] string Name,
    [MaxLength(50)] string? SKU,
    [MaxLength(50)] string? Barcode,
    [MaxLength(1000)] string? Description,
    decimal? CostPrice,
    [Required] decimal SalesPrice,
    [Required] double Quantity,
    int? MinStockLevel,
    int? MaxStockLevel,
    [MaxLength(500)] string? ImageUrl,
    decimal? Weight,
    [MaxLength(50)] string? Dimensions,
    [MaxLength(20)] string? UnitOfMeasure,
    [Required] Guid CategoryId,
    [Required] [MaxLength(50)] string CategoryName,
    Guid? SupplierId,
    [MaxLength(50)] string? SupplierName,
    [Required] bool IsActive);