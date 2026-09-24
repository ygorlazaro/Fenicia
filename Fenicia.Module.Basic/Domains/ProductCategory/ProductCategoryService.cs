using Fenicia.Common;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.DTOs.Basic.ProductCategory;
using Fenicia.Module.Basic.Domains.ProductCategory.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.ProductCategory;

/// <summary>
/// Service implementation for managing product categories.
/// </summary>
/// <param name="repository">The product category repository.</param>
public sealed class ProductCategoryService(IProductCategoryRepository repository) : IProductCategoryService
{
    /// <inheritdoc />
    public async Task<Pagination<List<ProductCategoryResponse>>> GetAllAsync(
        ProductCategoryQuery query,
        CancellationToken cancellationToken = default)
    {
        var baseQuery = repository.Query()
            .Where(pc => pc.Deleted == null);

        var total = await baseQuery.CountAsync(cancellationToken);

        var categories = await baseQuery
            .Select(pc => new ProductCategoryResponse(pc.Id, pc.Name))
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(cancellationToken);

        return new Pagination<List<ProductCategoryResponse>>(categories, total, query);
    }

    /// <inheritdoc />
    public async Task<ProductCategoryResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var category = await repository.GetByIdAsync(id, cancellationToken);

        return category is null ? null : ProductCategoryMapper.MapToResponse(category);
    }

    /// <inheritdoc />
    public async Task<ProductCategoryResponse> AddAsync(
        ProductCategoryRequest command,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        var category = new ProductCategoryModel
        {
            Name = command.Name,
            CompanyId = companyId
        };

        await repository.InsertAsync(category, cancellationToken);

        return ProductCategoryMapper.MapToResponse(category);
    }

    /// <inheritdoc />
    public async Task<ProductCategoryResponse?> UpdateAsync(
        Guid id,
        ProductCategoryRequest command,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        var category = await repository.GetByIdAsync(id, cancellationToken);

        if (category is null)
        {
            return null;
        }

        category.Name = command.Name;

        await repository.UpdateAsync(id, category, cancellationToken);

        return ProductCategoryMapper.MapToResponse(category);
    }

    /// <inheritdoc />
    public Task<List<ProductCategoryResponse>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken = default)
    {
        var idList = ids.ToList();
        return repository.Query()
            .Where(pc => idList.Contains(pc.Id))
            .Select(pc => new ProductCategoryResponse(pc.Id, pc.Name))
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(
        Guid id,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        await repository.DeleteAsync(id, cancellationToken);
    }
}
