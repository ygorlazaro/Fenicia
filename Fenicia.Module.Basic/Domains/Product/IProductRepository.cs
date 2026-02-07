using Fenicia.Common.Data.Abstracts;
using Fenicia.Common.Data.Models.Basic;

namespace Fenicia.Module.Basic.Domains.Product;

public interface IProductRepository : IBaseRepository<ProductModel>
{
    Task<List<ProductModel>> GetByCategoryIdAsync(Guid categoryId, CancellationToken ct, int page, int perPage);

    Task IncreaseStockAsync(Guid productId, double quantity, CancellationToken ct);

    Task DecreastStockAsync(Guid productId, double quantity, CancellationToken ct);

    Task<List<ProductModel>> GetInventoryAsync(Guid productId, CancellationToken ct, int page, int perPage);

    Task<decimal> GetTotalCostPriceByProductAsync(Guid productId, CancellationToken ct);

    Task<decimal> GetTotalSalesPriceProductAsync(Guid productId, CancellationToken ct);

    Task<double> GetTotalQuantityProductAsync(Guid productId, CancellationToken ct);

    Task<List<ProductModel>> GetInventoryByCategoryAsync(Guid categoryId, CancellationToken ct, int page, int perPage);

    Task<decimal> GetTotalCostPriceByCategoryAsync(Guid categoryId, CancellationToken ct);

    Task<decimal> GetTotalSalesPriceCategoryAsync(Guid categoryId, CancellationToken ct);

    Task<double> GetTotalQuantityCategoryAsync(Guid categoryId, CancellationToken ct);

    Task<List<ProductModel>> GetInventoryAsync(CancellationToken ct, int page, int perPage);

    Task<decimal> GetTotalCostPriceAsync(CancellationToken ct);

    Task<decimal> GetTotalSalesPriceAsync(CancellationToken ct);

    Task<double> GetTotalQuantityAsync(CancellationToken ct);
}