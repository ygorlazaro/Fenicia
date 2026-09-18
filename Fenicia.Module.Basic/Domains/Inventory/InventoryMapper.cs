using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.DTOs.Basic.Inventory;
using Riok.Mapperly.Abstractions;

namespace Fenicia.Module.Basic.Domains.Inventory;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
public partial class InventoryMapper
{
    [MapProperty("Category.Name", nameof(InventoryDetailResponse.CategoryName))]
    public partial InventoryDetailResponse MapToInventoryDetailResponse(ProductModel product);

    [MapProperty("Category.Name", nameof(InventoryDashboardItemResponse.CategoryName))]
    public partial InventoryDashboardItemResponse MapToInventoryDashboardItemResponse(ProductModel product);
}
