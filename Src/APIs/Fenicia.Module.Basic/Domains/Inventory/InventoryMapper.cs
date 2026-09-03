using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Inventory.DTOs;
using Riok.Mapperly.Abstractions;

namespace Fenicia.Module.Basic.Domains.Inventory;

[Mapper]
public static partial class InventoryMapper
{
    public static InventoryDetailResponse MapToInventoryDetailResponse(this ProductModel product)
    {
        return new InventoryDetailResponse(
            product.Id,
            product.Name,
            product.Quantity,
            product.CostPrice,
            product.SalesPrice,
            product.CategoryId,
            product.Category.Name);
    }

    public static InventoryDashboardItemResponse MapToInventoryDashboardItemResponse(this ProductModel product)
    {
        return new InventoryDashboardItemResponse(
            product.Id,
            product.Name,
            product.Quantity,
            product.CostPrice,
            product.SalesPrice,
            product.CategoryId,
            product.Category.Name);
    }
}