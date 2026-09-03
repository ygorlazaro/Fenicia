using Fenicia.Common;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.ProductCategory.DTOs;
using Fenicia.Module.Basic.Domains.ProductCategory.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.ProductCategory;

public sealed class ProductCategoryService(IProductCategoryRepository productCategoryRepository) : IProductCategoryService
{
    public ProductCategoryService()
        : this(null!)
    {
    }

    public async Task<Pagination<List<GetAllProductCategoryResponse>>> GetAllAsync(
        GetAllProductCategoryQuery query,
        CancellationToken cancellationToken = default)
    {
        var baseQuery = productCategoryRepository.Query();

        var filters = AdvancedQueryParser.Parse(query.Query);
        var filteredQuery = baseQuery.ApplyAdvancedQuery(filters, query.Sort);

        var total = await filteredQuery.CountAsync(cancellationToken);

        var categories = await filteredQuery
            .Select(pc => pc.MapToGetAllProductCategoryResponse())
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(cancellationToken);

        return new Pagination<List<GetAllProductCategoryResponse>>(categories, total, query.Page, query.PerPage);
    }

    public async Task<GetProductCategoryByIdResponse?> GetByIdAsync(
        GetProductCategoryByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var category = await productCategoryRepository.GetByIdAsync(query.Id, cancellationToken);

        return category?.MapToGetProductCategoryByIdResponse();
    }

    public async Task<AddProductCategoryResponse> AddAsync(
        AddProductCategoryCommand command,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        var category = new ProductCategoryModel
        {
            Id = command.Id,
            Name = command.Name,
            CompanyId = companyId
        };

        await productCategoryRepository.InsertAsync(category, cancellationToken);

        return category.MapToAddProductCategoryResponse();
    }

    public async Task<UpdateProductCategoryResponse?> UpdateAsync(
        UpdateProductCategoryCommand command,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        var category = await productCategoryRepository.GetByIdAsync(command.Id, cancellationToken);

        if (category is null)
        {
            return null;
        }

        category.Name = command.Name;
        category.CompanyId = companyId;

        await productCategoryRepository.UpdateAsync(command.Id, category, cancellationToken);

        return category.MapToUpdateProductCategoryResponse();
    }

    public Task<List<GetProductCategoryByIdResponse>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken = default)
    {
        var idList = ids.ToList();
        return productCategoryRepository.Query()
            .Where(pc => idList.Contains(pc.Id))
            .Select(pc => pc.MapToGetProductCategoryByIdResponse())
            .ToListAsync(cancellationToken);
    }

    public async Task DeleteAsync(
        DeleteProductCategoryCommand command,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        await productCategoryRepository.DeleteAsync(command.Id, cancellationToken);
    }
}