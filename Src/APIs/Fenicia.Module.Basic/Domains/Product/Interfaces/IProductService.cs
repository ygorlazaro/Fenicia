using System.Linq.Expressions;
using Fenicia.Common;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.DataSource.DTOs;
using Fenicia.Module.Basic.Domains.Inventory.DTOs;
using Fenicia.Module.Basic.Domains.Product.DTOs;

namespace Fenicia.Module.Basic.Domains.Product.Interfaces;

public interface IProductService
{
    Task<Pagination<List<GetAllProductResponse>>> GetAllAsync(
        GetAllProductQuery query,
        CancellationToken cancellationToken = default);

    Task<List<GetAllProductForDataSourceResponse>> GetAllForDataSourceAsync(
        CancellationToken cancellationToken = default);

    Task<List<Fenicia.Module.Basic.Domains.DataSource.DTOs.GetAllDashboardProductForDataSourceResponse>>
        GetAllDashboardForDataSourceAsync(
            CancellationToken cancellationToken = default);

    Task<GetProductByIdResponse?> GetByIdAsync(
        GetProductByIdQuery query,
        CancellationToken cancellationToken = default);

    Task<List<GetProductsByCategoryIdResponse>> GetByCategoryIdAsync(
        GetProductsByCategoryIdQuery query,
        int page = 1,
        int perPage = 10,
        CancellationToken cancellationToken = default);

    Task<AddProductResponse> AddAsync(
        AddProductCommand command,
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task<UpdateProductResponse?> UpdateAsync(
        UpdateProductCommand command,
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(DeleteProductCommand command, Guid companyId, CancellationToken cancellationToken = default);

    Task<ProductPerformanceResponse> GetPerformanceAsync(
        GetProductPerformanceQuery query,
        CancellationToken cancellationToken = default);

    Task<int> GetCountAsync(CancellationToken cancellationToken = default);

    Task<int> GetTotalProductsAsync(CancellationToken cancellationToken = default);

    Task<List<ProductModel>> GetAllWithSupplierAsync(CancellationToken cancellationToken = default);

    Task<List<ProductModel>> GetAllForStatsAsync(CancellationToken cancellationToken = default);

    Task<IEnumerable<ProductModel>> GetAllWithCategoryAsync(
        GetAllProductQuery query,
        CancellationToken cancellationToken = default);

    Task<decimal> GetTotalCostPriceAsync(CancellationToken cancellationToken = default);

    Task<decimal> GetTotalSalesPriceAsync(CancellationToken cancellationToken = default);

    Task<int> GetTotalQuantityAsync(CancellationToken cancellationToken = default);

    Task<IEnumerable<ProductModel>> GetByCategoryWithCategoryAsync(
        Guid categoryId,
        int page = 1,
        int perPage = 10,
        CancellationToken cancellationToken = default);

    Task<decimal> GetTotalCostPriceByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default);

    Task<decimal> GetTotalSalesPriceByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default);

    Task<int> GetTotalQuantityByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default);

    Task<IEnumerable<ProductModel>> GetByIdWithCategoryAsync(
        Guid productId,
        int page = 1,
        int perPage = 10,
        CancellationToken cancellationToken = default);

    Task<decimal> GetTotalCostPriceByProductAsync(Guid productId, CancellationToken cancellationToken = default);

    Task<decimal> GetTotalSalesPriceByProductAsync(Guid productId, CancellationToken cancellationToken = default);

    Task<int> GetTotalQuantityByProductAsync(Guid productId, CancellationToken cancellationToken = default);

    Task<List<ProductModel>> GetLowStockAsync(CancellationToken cancellationToken = default);

    Task<decimal> GetTotalCostValueAsync(CancellationToken cancellationToken = default);

    Task<decimal> GetTotalSalesValueAsync(CancellationToken cancellationToken = default);

    Task<List<ProductModel>> GetZeroMovementCandidatesAsync(
        IEnumerable<Guid> activeProductIds,
        CancellationToken cancellationToken = default);

    Task<List<ProductModel>> GetOverstockCandidatesAsync(CancellationToken cancellationToken = default);

    Task<int> CountAsync(Expression<Func<ProductModel, bool>> predicate, CancellationToken cancellationToken = default);

    Task<List<(Guid CategoryId, string CategoryName, int Quantity, decimal? CostPrice)>> GetStockValueByCategoryAsync(
        CancellationToken cancellationToken = default);

    Task<List<CategoryBreakdownResponse>> GetCategoryBreakdownAsync(CancellationToken cancellationToken = default);
}