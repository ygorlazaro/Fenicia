using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Repositories;

namespace Fenicia.Module.Basic.Domains.Product;

public interface IProductRepository : IRepository<ProductModel>
{
    IQueryable<ProductModel> Query();

    Task<IEnumerable<ProductModel>> GetAllWithDetailsAsync(int page = 1, int perPage = 10, CancellationToken ct);

    Task<ProductModel?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct);

    Task<IEnumerable<ProductModel>> GetByCategoryIdAsync(Guid categoryId, int page = 1, int perPage = 10, CancellationToken ct);

    Task<IEnumerable<ProductModel>> GetAllWithCategoryAsync(int page = 1, int perPage = 10, CancellationToken ct);

    Task<IEnumerable<ProductModel>> GetByCategoryWithCategoryAsync(Guid categoryId, int page = 1, int perPage = 10, CancellationToken ct);

    Task<IEnumerable<ProductModel>> GetByIdWithCategoryAsync(Guid productId, int page = 1, int perPage = 10, CancellationToken ct);

    Task<List<ProductModel>> GetLowStockAsync(CancellationToken ct);

    Task<decimal> GetTotalCostPriceAsync(CancellationToken ct);

    Task<decimal> GetTotalSalesPriceAsync(CancellationToken ct);

    Task<int> GetTotalQuantityAsync(CancellationToken ct);

    Task<decimal> GetTotalCostPriceByCategoryAsync(Guid categoryId, CancellationToken ct);

    Task<decimal> GetTotalSalesPriceByCategoryAsync(Guid categoryId, CancellationToken ct);

    Task<int> GetTotalQuantityByCategoryAsync(Guid categoryId, CancellationToken ct);

    Task<decimal> GetTotalCostPriceByProductAsync(Guid productId, CancellationToken ct);

    Task<decimal> GetTotalSalesPriceByProductAsync(Guid productId, CancellationToken ct);

    Task<int> GetTotalQuantityByProductAsync(Guid productId, CancellationToken ct);

    Task<decimal> GetTotalCostValueAsync(CancellationToken ct);

    Task<decimal> GetTotalSalesValueAsync(CancellationToken ct);

    Task<List<ProductModel>> GetZeroMovementCandidatesAsync(IEnumerable<Guid> activeProductIds, CancellationToken ct);

    Task<List<ProductModel>> GetOverstockCandidatesAsync(CancellationToken ct);

    Task<List<(Guid CategoryId, string CategoryName, int Quantity, decimal? CostPrice)>> GetStockValueByCategoryAsync(CancellationToken ct);
}
