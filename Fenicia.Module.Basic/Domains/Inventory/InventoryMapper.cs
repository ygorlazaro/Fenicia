using Fenicia.Common.DTOs.Basic.Inventory;
using Fenicia.Common.Data.Models.Basic;

namespace Fenicia.Module.Basic.Domains.Inventory;

/// <summary>
/// Mapper class for mapping inventory data.
/// </summary>
public static class InventoryMapper
{
    /// <summary>
    /// Maps a ProductModel to an InventoryDetailResponse.
    /// </summary>
    /// <param name="product">The product model.</param>
    /// <returns>The inventory detail response.</returns>
    public static InventoryDetailResponse MapToDetailResponse(ProductModel product)
    {
        return new InventoryDetailResponse(
            product.Id,
            product.Name,
            product.Quantity,
            product.CostPrice,
            product.SalesPrice,
            product.CategoryId,
            product.Category?.Name ?? string.Empty);
    }
}