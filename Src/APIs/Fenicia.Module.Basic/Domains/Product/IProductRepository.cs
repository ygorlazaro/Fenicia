using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Repositories;

namespace Fenicia.Module.Basic.Domains.Product;

public interface IProductRepository : IRepository<ProductModel>
{
    IQueryable<ProductModel> Query();

    Task<IEnumerable<ProductModel>> GetAllWithDetailsAsync(int page = 1, int perPage = 10, CancellationToken ct = default);

    Task<ProductModel?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default);

    Task<IEnumerable<ProductModel>> GetByCategoryIdAsync(Guid categoryId, int page = 1, int perPage = 10, CancellationToken ct = default);

    Task<IEnumerable<ProductModel>> GetAllWithCategoryAsync(int page = 1, int perPage = 10, CancellationToken ct = default);

    Task<IEnumerable<ProductModel>> GetByCategoryWithCategoryAsync(Guid categoryId, int page = 1, int perPage = 10, CancellationToken ct = default);

    Task<IEnumerable<ProductModel>> GetByIdWithCategoryAsync(Guid productId, int page = 1, int perPage = 10, CancellationToken ct = default);

    Task<List<ProductModel>> GetLowStockAsync(CancellationToken ct = default);

    Task<decimal> GetTotalCostPriceAsync(CancellationToken ct = default);

    Task<decimal> GetTotalSalesPriceAsync(CancellationToken ct = default);

    Task<int> GetTotalQuantityAsync(CancellationToken ct = default);

    Task<decimal> GetTotalCostPriceByCategoryAsync(Guid categoryId, CancellationToken ct = default);

    Task<decimal> GetTotalSalesPriceByCategoryAsync(Guid categoryId, CancellationToken ct = default);

    Task<int> GetTotalQuantityByCategoryAsync(Guid categoryId, CancellationToken ct = default);

    Task<decimal> GetTotalCostPriceByProductAsync(Guid productId, CancellationToken ct = default);

    Task<decimal> GetTotalSalesPriceByProductAsync(Guid productId, CancellationToken ct = default);

    Task<int> GetTotalQuantityByProductAsync(Guid productId, CancellationToken ct = default);

    Task<decimal> GetTotalCostValueAsync(CancellationToken ct = default);

    Task<decimal> GetTotalSalesValueAsync(CancellationToken ct = default);

    Task<List<ProductModel>> GetZeroMovementCandidatesAsync(IEnumerable<Guid> activeProductIds, CancellationToken ct = default);

    Task<List<ProductModel>> GetOverstockCandidatesAsync(CancellationToken ct = default);

    Task<List<(Guid CategoryId, string CategoryName, int Quantity, decimal? CostPrice)>> GetStockValueByCategoryAsync(CancellationToken ct = default);
}