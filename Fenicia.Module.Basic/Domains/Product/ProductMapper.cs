using Fenicia.Common.DTOs.Basic.Product;
using Fenicia.Common.Data.Models.Basic;

namespace Fenicia.Module.Basic.Domains.Product;

/// <summary>
/// Mapper class for mapping product data.
/// </summary>
public static class ProductMapper
{
    /// <summary>
    /// Maps a ProductModel to a ProductResponse.
    /// </summary>
    /// <param name="product">The product model.</param>
    /// <returns>The product response.</returns>
    public static ProductResponse MapToResponse(ProductModel product)
    {
        return new ProductResponse(
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
            product.Supplier?.Person.Name,
            product.IsActive);
    }

    /// <summary>
    /// Maps a collection of ProductModel to a list of ProductResponse.
    /// </summary>
    /// <param name="products">The product models.</param>
    /// <returns>A list of product responses.</returns>
    public static List<ProductResponse> MapToResponseList(IEnumerable<ProductModel> products)
    {
        return [.. products.Select(MapToResponse)];
    }

    /// <summary>
    /// Maps a ProductResponse to a ProductModel.
    /// </summary>
    /// <param name="response">The product response.</param>
    /// <returns>The product model.</returns>
    public static ProductModel MapToModel(ProductResponse response)
    {
        return new ProductModel
        {
            Id = response.Id,
            Name = response.Name,
            SKU = response.SKU,
            Barcode = response.Barcode,
            Description = response.Description,
            CostPrice = response.CostPrice,
            SalesPrice = response.SalesPrice,
            Quantity = response.Quantity,
            MinStockLevel = response.MinStockLevel,
            MaxStockLevel = response.MaxStockLevel,
            ImageUrl = response.ImageUrl,
            Weight = response.Weight,
            Dimensions = response.Dimensions,
            UnitOfMeasure = response.UnitOfMeasure,
            CategoryId = response.CategoryId,
            SupplierId = response.SupplierId,
            IsActive = response.IsActive
        };
    }
}