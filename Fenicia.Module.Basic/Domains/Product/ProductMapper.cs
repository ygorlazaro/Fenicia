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
}