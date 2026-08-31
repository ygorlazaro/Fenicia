using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Repositories;

namespace Fenicia.Module.Basic.Domains.Product;

public interface IProductRepository : IRepository<ProductModel>
{
    IQueryable<ProductModel> Query();

    Task<IEnumerable<ProductModel>> GetAllWithDetailsAsync(int page = 1, int perPage = 10, CancellationToken cancellationToken = default);

    Task<ProductModel?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IEnumerable<ProductModel>> GetByCategoryIdAsync(Guid categoryId, int page = 1, int perPage = 10, CancellationToken cancellationToken = default);

    Task<IEnumerable<ProductModel>> GetAllWithCategoryAsync(int page = 1, int perPage = 10, CancellationToken cancellationToken = default);

    Task<IEnumerable<ProductModel>> GetByCategoryWithCategoryAsync(Guid categoryId, int page = 1, int perPage = 10, CancellationToken cancellationToken = default);

    Task<IEnumerable<ProductModel>> GetByIdWithCategoryAsync(Guid productId, int page = 1, int perPage = 10, CancellationToken cancellationToken = default);

    Task<List<ProductModel>> GetLowStockAsync(CancellationToken cancellationToken = default);

    Task<decimal> GetTotalCostPriceAsync(CancellationToken cancellationToken = default);

    Task<decimal> GetTotalSalesPriceAsync(CancellationToken cancellationToken = default);

    Task<int> GetTotalQuantityAsync(CancellationToken cancellationToken = default);

    Task<decimal> GetTotalCostPriceByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default);

    Task<decimal> GetTotalSalesPriceByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default);

    Task<int> GetTotalQuantityByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default);

    Task<decimal> GetTotalCostPriceByProductAsync(Guid productId, CancellationToken cancellationToken = default);

    Task<decimal> GetTotalSalesPriceByProductAsync(Guid productId, CancellationToken cancellationToken = default);

    Task<int> GetTotalQuantityByProductAsync(Guid productId, CancellationToken cancellationToken = default);

    Task<decimal> GetTotalCostValueAsync(CancellationToken cancellationToken = default);

    Task<decimal> GetTotalSalesValueAsync(CancellationToken cancellationToken = default);

    Task<List<ProductModel>> GetZeroMovementCandidatesAsync(IEnumerable<Guid> activeProductIds, CancellationToken cancellationToken = default);

    Task<List<ProductModel>> GetOverstockCandidatesAsync(CancellationToken cancellationToken = default);

    Task<List<(Guid CategoryId, string CategoryName, int Quantity, decimal? CostPrice)>> GetStockValueByCategoryAsync(CancellationToken cancellationToken = default);
}
