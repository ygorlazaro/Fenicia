using Fenicia.Common.Data.Abstracts;
using Fenicia.Common.Data.Models.Basic;

namespace Fenicia.Module.Basic.Domains.Product;

public interface IProductRepository : IBaseRepository<ProductModel>
{
    Task<List<ProductModel>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken, int page, int perPage);

    Task IncreaseStockAsync(Guid productId, double quantity, CancellationToken cancellationToken);

    Task DecreastStockAsync(Guid productId, double quantity, CancellationToken cancellationToken);

    Task<List<ProductModel>> GetInventoryAsync(Guid productId, CancellationToken cancellationToken, int page, int perPage);

    Task<decimal> GetTotalCostPriceByProductAsync(Guid productId, CancellationToken cancellationToken);

    Task<decimal> GetTotalSalesPriceProductAsync(Guid productId, CancellationToken cancellationToken);

    Task<double> GetTotalQuantityProductAsync(Guid productId, CancellationToken cancellationToken);

    Task<List<ProductModel>> GetInventoryByCategoryAsync(Guid categoryId, CancellationToken cancellationToken, int page, int perPage);

    Task<decimal> GetTotalCostPriceByCategoryAsync(Guid categoryId, CancellationToken cancellationToken);

    Task<decimal> GetTotalSalesPriceCategoryAsync(Guid categoryId, CancellationToken cancellationToken);

    Task<double> GetTotalQuantityCategoryAsync(Guid categoryId, CancellationToken cancellationToken);

    Task<List<ProductModel>> GetInventoryAsync(CancellationToken cancellationToken, int page, int perPage);

    Task<decimal> GetTotalCostPriceAsync(CancellationToken cancellationToken);

    Task<decimal> GetTotalSalesPriceAsync(CancellationToken cancellationToken);

    Task<double> GetTotalQuantityAsync(CancellationToken cancellationToken);
}
