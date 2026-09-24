using Fenicia.Common;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Repositories;
using Fenicia.Common.DTOs.Basic.Inventory;

namespace Fenicia.Module.Basic.Domains.Product.Interfaces;

/// <summary>
/// Repository interface for managing product data.
/// </summary>
public interface IProductRepository : IRepository<ProductModel>
{
    /// <summary>
    /// Gets all products with details.
    /// </summary>
    /// <param name="query">The pagination and filter query.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A collection of product models.</returns>
    Task<IEnumerable<ProductModel>> GetAllWithDetailsAsync(
        PaginationQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a product by ID with details.
    /// </summary>
    /// <param name="id">The product ID.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The product model if found, or null.</returns>
    Task<ProductModel?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets products by category ID.
    /// </summary>
    /// <param name="categoryId">The category ID.</param>
    /// <param name="query">The pagination and filter query.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A collection of product models.</returns>
    Task<IEnumerable<ProductModel>> GetByCategoryIdAsync(
        Guid categoryId,
        PaginationQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all products with category.
    /// </summary>
    /// <param name="query">The pagination and filter query.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A collection of product models.</returns>
    Task<IEnumerable<ProductModel>> GetAllWithCategoryAsync(
        PaginationQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets products by category with category details.
    /// </summary>
    /// <param name="categoryId">The category ID.</param>
    /// <param name="query">The pagination and filter query.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A collection of product models.</returns>
    Task<IEnumerable<ProductModel>> GetByCategoryWithCategoryAsync(
        Guid categoryId,
        PaginationQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a product by ID with category.
    /// </summary>
    /// <param name="productId">The product ID.</param>
    /// <param name="query">The pagination and filter query.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A collection of product models.</returns>
    Task<IEnumerable<ProductModel>> GetByIdWithCategoryAsync(
        Guid productId,
        PaginationQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets low stock products.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of product models.</returns>
    Task<List<ProductModel>> GetLowStockAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets total cost price.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The total cost price.</returns>
    Task<decimal> GetTotalCostPriceAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets total sales price.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The total sales price.</returns>
    Task<decimal> GetTotalSalesPriceAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets total quantity.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The total quantity.</returns>
    Task<int> GetTotalQuantityAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets total cost price by category.
    /// </summary>
    /// <param name="categoryId">The category ID.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The total cost price.</returns>
    Task<decimal> GetTotalCostPriceByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets total sales price by category.
    /// </summary>
    /// <param name="categoryId">The category ID.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The total sales price.</returns>
    Task<decimal> GetTotalSalesPriceByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets total quantity by category.
    /// </summary>
    /// <param name="categoryId">The category ID.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The total quantity.</returns>
    Task<int> GetTotalQuantityByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets total cost price by product.
    /// </summary>
    /// <param name="productId">The product ID.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The total cost price.</returns>
    Task<decimal> GetTotalCostPriceByProductAsync(Guid productId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets total sales price by product.
    /// </summary>
    /// <param name="productId">The product ID.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The total sales price.</returns>
    Task<decimal> GetTotalSalesPriceByProductAsync(Guid productId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets total quantity by product.
    /// </summary>
    /// <param name="productId">The product ID.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The total quantity.</returns>
    Task<int> GetTotalQuantityByProductAsync(Guid productId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets total cost value.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The total cost value.</returns>
    Task<decimal> GetTotalCostValueAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets total sales value.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The total sales value.</returns>
    Task<decimal> GetTotalSalesValueAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets zero movement candidates.
    /// </summary>
    /// <param name="activeProductIds">The active product IDs.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of product models.</returns>
    Task<List<ProductModel>> GetZeroMovementCandidatesAsync(
        IEnumerable<Guid> activeProductIds,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets stock value by category.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of category stock values.</returns>
    Task<List<(Guid CategoryId, string CategoryName, int Quantity, decimal? CostPrice)>> GetStockValueByCategoryAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all products with supplier.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of product models.</returns>
    Task<List<ProductModel>> GetAllWithSupplierAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all products for stats.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of product models.</returns>
    Task<List<ProductModel>> GetAllForStatsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets category breakdown.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of category breakdown responses.</returns>
    Task<List<CategoryBreakdownResponse>> GetCategoryBreakdownAsync(CancellationToken cancellationToken = default);
}