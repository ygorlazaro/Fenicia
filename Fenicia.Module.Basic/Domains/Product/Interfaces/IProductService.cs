using System.Linq.Expressions;
using Fenicia.Common;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.DTOs.Basic.DataSource;
using Fenicia.Common.DTOs.Basic.Inventory;
using Fenicia.Common.DTOs.Basic.Product;

namespace Fenicia.Module.Basic.Domains.Product.Interfaces;

/// <summary>
/// Service interface for product operations.
/// </summary>
public interface IProductService
{
    /// <summary>
    /// Gets a paginated list of products.
    /// </summary>
    /// <param name="query">The pagination and filter query.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A paginated list of product responses.</returns>
    Task<Pagination<List<ProductResponse>>> GetAllAsync(
        ProductQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all products for data source (dropdowns).
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of product data source responses.</returns>
    Task<List<ProductDataSourceResponse>> GetAllForDataSourceAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all products for dashboard data source.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of dashboard product data source responses.</returns>
    Task<List<DashboardProductDataSourceResponse>>
        GetAllDashboardForDataSourceAsync(
            CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a product by ID.
    /// </summary>
    /// <param name="id">The product ID.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The product response if found, or null.</returns>
    Task<ProductResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets products by category ID.
    /// </summary>
    /// <param name="request">The category query request.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A paginated list of product responses.</returns>
    Task<Pagination<List<ProductResponse>>> GetByCategoryIdAsync(
        ProductsByCategoryIdRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new product.
    /// </summary>
    /// <param name="command">The product request data.</param>
    /// <param name="companyId">The company ID.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The created product response.</returns>
    Task<ProductResponse> AddAsync(
        ProductRequest command,
        Guid companyId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing product.
    /// </summary>
    /// <param name="id">The product ID.</param>
    /// <param name="command">The product request data.</param>
    /// <param name="companyId">The company ID.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The updated product response if found, or null.</returns>
    Task<ProductResponse?> UpdateAsync(
        Guid id,
        ProductRequest command,
        Guid companyId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a product.
    /// </summary>
    /// <param name="id">The product ID.</param>
    /// <param name="companyId">The company ID.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteAsync(
        Guid id,
        Guid companyId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets product performance metrics.
    /// </summary>
    /// <param name="request">The performance request parameters.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The product performance response.</returns>
    Task<ProductPerformanceResponse> GetPerformanceAsync(
        ProductPerformanceRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total product count.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The total product count.</returns>
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total product count (alias for GetCountAsync).
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The total product count.</returns>
    Task<int> GetTotalProductsAsync(CancellationToken cancellationToken = default);

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
    /// Gets category breakdown.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of category breakdown responses.</returns>
    Task<List<CategoryBreakdownResponse>> GetCategoryBreakdownAsync(CancellationToken cancellationToken = default);
}