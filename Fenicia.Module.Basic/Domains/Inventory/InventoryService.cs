using Fenicia.Common.Data.Responses.Basic;
using Fenicia.Module.Basic.Domains.Product;

namespace Fenicia.Module.Basic.Domains.Inventory;

public class InventoryService(IProductRepository productRepository) : IInventoryService
{
    public async Task<InventoryResponse> GetInventoryByProductAsync(Guid productId, CancellationToken ct, int page = 1, int perPage = 10)
    {
        var productInventory = await productRepository.GetInventoryAsync(productId, ct, page, perPage);

        var inventory = InventoryResponse.Map(productInventory);
        inventory.TotalCostPrice = await productRepository.GetTotalCostPriceByProductAsync(productId, ct);
        inventory.TotalSalesPrice = await productRepository.GetTotalSalesPriceProductAsync(productId, ct);
        inventory.TotalQuantity = await productRepository.GetTotalQuantityProductAsync(productId, ct);

        return inventory;
    }

    public async Task<InventoryResponse> GetInventoryByCategoryAsync(Guid categoryId, CancellationToken ct, int page = 1, int perPage = 10)
    {
        var productInventory = await productRepository.GetInventoryByCategoryAsync(categoryId, ct, page, perPage);

        var inventory = InventoryResponse.Map(productInventory);
        inventory.TotalCostPrice = await productRepository.GetTotalCostPriceByCategoryAsync(categoryId, ct);
        inventory.TotalSalesPrice = await productRepository.GetTotalSalesPriceCategoryAsync(categoryId, ct);
        inventory.TotalQuantity = await productRepository.GetTotalQuantityCategoryAsync(categoryId, ct);

        return inventory;
    }

    public async Task<InventoryResponse> GetInventoryAsync(CancellationToken ct, int page = 1, int perPage = 10)
    {
        var productInventory = await productRepository.GetInventoryAsync(ct, page, perPage);

        var inventory = InventoryResponse.Map(productInventory);
        inventory.TotalCostPrice = await productRepository.GetTotalCostPriceAsync(ct);
        inventory.TotalSalesPrice = await productRepository.GetTotalSalesPriceAsync(ct);
        inventory.TotalQuantity = await productRepository.GetTotalQuantityAsync(ct);

        return inventory;
    }
}
