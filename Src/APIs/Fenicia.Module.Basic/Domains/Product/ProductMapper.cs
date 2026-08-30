using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Product.DTOs;
using Riok.Mapperly.Abstractions;

namespace Fenicia.Module.Basic.Domains.Product;

[Mapper]
public static partial class ProductMapper
{
    public static GetAllProductResponse MapToGetAllProductResponse(this ProductModel product)
    {
        return new GetAllProductResponse(
            product.Id,
            product.Name,
            product.SKU,
            product.Barcode,
            product.Description,
            product.CostPrice,
            product.SalesPrice,
            product.Quantity,
            product.MinStockLevel,
            product.MaxStockLevel,
            product.ImageUrl,
            product.Weight,
            product.Dimensions,
            product.UnitOfMeasure,
            product.CategoryId,
            product.Category?.Name ?? string.Empty,
            product.SupplierId,
            product.Supplier?.Person?.Name ?? string.Empty,
            product.IsActive);
    }

    public static GetProductByIdResponse MapToGetProductByIdResponse(this ProductModel product)
    {
        return new GetProductByIdResponse(
            product.Id,
            product.Name,
            product.SKU,
            product.Barcode,
            product.Description,
            product.CostPrice,
            product.SalesPrice,
            product.Quantity,
            product.MinStockLevel,
            product.MaxStockLevel,
            product.ImageUrl,
            product.Weight,
            product.Dimensions,
            product.UnitOfMeasure,
            product.CategoryId,
            product.Category?.Name ?? string.Empty,
            product.SupplierId,
            product.Supplier?.Person?.Name,
            product.IsActive);
    }

    public static GetProductsByCategoryIdResponse MapToGetProductsByCategoryIdResponse(this ProductModel product)
    {
        return new GetProductsByCategoryIdResponse(
            product.Id,
            product.Name,
            product.SKU,
            product.Barcode,
            product.Description,
            product.CostPrice,
            product.SalesPrice,
            product.Quantity,
            product.MinStockLevel,
            product.MaxStockLevel,
            product.ImageUrl,
            product.Weight,
            product.Dimensions,
            product.UnitOfMeasure,
            product.CategoryId,
            product.Category.Name,
            product.IsActive);
    }

    public static AddProductResponse MapToAddProductResponse(this ProductModel product, string categoryName, string? supplierName)
    {
        return new AddProductResponse(
            product.Id,
            product.Name,
            product.SKU,
            product.Barcode,
            product.Description,
            product.CostPrice,
            product.SalesPrice,
            product.Quantity,
            product.MinStockLevel,
            product.MaxStockLevel,
            product.ImageUrl,
            product.Weight,
            product.Dimensions,
            product.UnitOfMeasure,
            product.CategoryId,
            categoryName,
            product.SupplierId,
            supplierName,
            product.IsActive);
    }

    public static UpdateProductResponse MapToUpdateProductResponse(this ProductModel product, string categoryName, string? supplierName)
    {
        return new UpdateProductResponse(
            product.Id,
            product.Name,
            product.SKU,
            product.Barcode,
            product.Description,
            product.CostPrice,
            product.SalesPrice,
            product.Quantity,
            product.MinStockLevel,
            product.MaxStockLevel,
            product.ImageUrl,
            product.Weight,
            product.Dimensions,
            product.UnitOfMeasure,
            product.CategoryId,
            categoryName,
            product.SupplierId,
            supplierName,
            product.IsActive);
    }
}
