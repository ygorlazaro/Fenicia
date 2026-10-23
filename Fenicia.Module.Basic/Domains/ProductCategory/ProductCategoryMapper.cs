using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.DTOs.Basic.ProductCategory;

namespace Fenicia.Module.Basic.Domains.ProductCategory;

/// <summary>
/// Mapper class for mapping product category data.
/// </summary>
public static class ProductCategoryMapper
{
    /// <summary>
    /// Maps a ProductCategoryModel to a ProductCategoryResponse.
    /// </summary>
    /// <param name="category">The product category model.</param>
    /// <returns>The product category response.</returns>
    public static ProductCategoryResponse MapToResponse(ProductCategoryModel category)
    {
        return new ProductCategoryResponse(category.Id, category.Name);
    }

    /// <summary>
    /// Maps a collection of ProductCategoryModel to a list of ProductCategoryResponse.
    /// </summary>
    /// <param name="categories">The product category models.</param>
    /// <returns>A list of product category responses.</returns>
    public static List<ProductCategoryResponse> MapToResponseList(IEnumerable<ProductCategoryModel> categories)
    {
        return [.. categories.Select(MapToResponse)];
    }
}