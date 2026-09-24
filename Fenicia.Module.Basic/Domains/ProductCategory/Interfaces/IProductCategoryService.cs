using Fenicia.Common;
using Fenicia.Common.DTOs.Basic.ProductCategory;

namespace Fenicia.Module.Basic.Domains.ProductCategory.Interfaces;

/// <summary>
/// Service interface for product category operations.
/// </summary>
public interface IProductCategoryService
{
    /// <summary>
    /// Gets a paginated list of product categories.
    /// </summary>
    /// <param name="query">The pagination and filter query.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A paginated list of product category responses.</returns>
    Task<Pagination<List<ProductCategoryResponse>>> GetAllAsync(
        ProductCategoryQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a product category by ID.
    /// </summary>
    /// <param name="id">The product category ID.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The product category response if found, or null.</returns>
    Task<ProductCategoryResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new product category.
    /// </summary>
    /// <param name="command">The product category request data.</param>
    /// <param name="companyId">The company ID.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The created product category response.</returns>
    Task<ProductCategoryResponse> AddAsync(
        ProductCategoryRequest command,
        Guid companyId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing product category.
    /// </summary>
    /// <param name="id">The product category ID.</param>
    /// <param name="command">The product category request data.</param>
    /// <param name="companyId">The company ID.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The updated product category response if found, or null.</returns>
    Task<ProductCategoryResponse?> UpdateAsync(
        Guid id,
        ProductCategoryRequest command,
        Guid companyId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets product categories by IDs.
    /// </summary>
    /// <param name="ids">The product category IDs.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of product category responses.</returns>
    Task<List<ProductCategoryResponse>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a product category.
    /// </summary>
    /// <param name="id">The product category ID.</param>
    /// <param name="companyId">The company ID.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteAsync(
        Guid id,
        Guid companyId,
        CancellationToken cancellationToken = default);
}