using Fenicia.Common.Data.Responses.Basic;
using Fenicia.Module.Basic.Domains.Product;

namespace Fenicia.Module.Basic.Domains.Inventory;

public class InventoryService(IProductRepository productRepository) : IInventoryService
{
    public async Task<InventoryResponse> GetInventoryByProductAsync(Guid productId, CancellationToken cancellationToken, int page = 1, int perPage = 10)
    {
        var productInventory = await productRepository.GetInventoryAsync(productId, cancellationToken, page, perPage);

        var inventory = InventoryResponse.Convert(productInventory);
        inventory.TotalCostPrice = await productRepository.GetTotalCostPriceByProductAsync(productId, cancellationToken);
        inventory.TotalSalesPrice = await productRepository.GetTotalSalesPriceProductAsync(productId, cancellationToken);
        inventory.TotalQuantity = await productRepository.GetTotalQuantityProductAsync(productId, cancellationToken);

        return inventory;
    }

    public async Task<InventoryResponse> GetInventoryByCategoryAsync(Guid categoryId, CancellationToken cancellationToken, int page = 1, int perPage = 10)
    {
        var productInventory = await productRepository.GetInventoryByCategoryAsync(categoryId, cancellationToken, page, perPage);

        var inventory = InventoryResponse.Convert(productInventory);
        inventory.TotalCostPrice = await productRepository.GetTotalCostPriceByCategoryAsync(categoryId, cancellationToken);
        inventory.TotalSalesPrice = await productRepository.GetTotalSalesPriceCategoryAsync(categoryId, cancellationToken);
        inventory.TotalQuantity = await productRepository.GetTotalQuantityCategoryAsync(categoryId, cancellationToken);

        return inventory;
    }

    public async Task<InventoryResponse> GetInventoryAsync(CancellationToken cancellationToken, int page = 1, int perPage = 10)
    {
        var productInventory = await productRepository.GetInventoryAsync(cancellationToken, page, perPage);

        var inventory = InventoryResponse.Convert(productInventory);
        inventory.TotalCostPrice = await productRepository.GetTotalCostPriceAsync(cancellationToken);
        inventory.TotalSalesPrice = await productRepository.GetTotalSalesPriceAsync(cancellationToken);
        inventory.TotalQuantity = await productRepository.GetTotalQuantityAsync(cancellationToken);

        return inventory;
    }
}
