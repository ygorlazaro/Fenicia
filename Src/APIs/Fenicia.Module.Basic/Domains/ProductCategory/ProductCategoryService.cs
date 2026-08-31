using Fenicia.Common;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.ProductCategory.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.ProductCategory;

public class ProductCategoryService
{
    private readonly IProductCategoryRepository _productCategoryRepository;

    public ProductCategoryService()
        : this(null!)
    {
    }

    public ProductCategoryService(IProductCategoryRepository productCategoryRepository)
    {
        _productCategoryRepository = productCategoryRepository;
    }

    public virtual async Task<Pagination<List<GetAllProductCategoryResponse>>> GetAllAsync(GetAllProductCategoryQuery query, CancellationToken cancellationToken = default)
    {
        var total = await _productCategoryRepository.CountAsync(cancellationToken);

        var categories = await _productCategoryRepository.Query()
            .Select(pc => pc.MapToGetAllProductCategoryResponse())
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(cancellationToken);

        return new Pagination<List<GetAllProductCategoryResponse>>(categories, total, query.Page, query.PerPage);
    }

    public virtual async Task<GetProductCategoryByIdResponse?> GetByIdAsync(GetProductCategoryByIdQuery query, CancellationToken cancellationToken = default)
    {
        var category = await _productCategoryRepository.GetByIdAsync(query.Id, cancellationToken);

        return category is null ? null : category.MapToGetProductCategoryByIdResponse();
    }

    public virtual async Task<AddProductCategoryResponse> AddAsync(AddProductCategoryCommand command, Guid companyId, CancellationToken cancellationToken = default)
    {
        var category = new ProductCategoryModel
        {
            Id = command.Id,
            Name = command.Name,
            CompanyId = companyId
        };

        await _productCategoryRepository.InsertAsync(category, cancellationToken);

        return category.MapToAddProductCategoryResponse();
    }

    public virtual async Task<UpdateProductCategoryResponse?> UpdateAsync(UpdateProductCategoryCommand command, Guid companyId, CancellationToken cancellationToken = default)
    {
        var category = await _productCategoryRepository.GetByIdAsync(command.Id, cancellationToken);

        if (category is null)
        {
            return null;
        }

        category.Name = command.Name;
        category.CompanyId = companyId;

        await _productCategoryRepository.UpdateAsync(command.Id, category, cancellationToken);

        return category.MapToUpdateProductCategoryResponse();
    }

    public virtual async Task<List<GetProductCategoryByIdResponse>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        var idList = ids.ToList();
        return await _productCategoryRepository.Query()
            .Where(pc => idList.Contains(pc.Id))
            .Select(pc => pc.MapToGetProductCategoryByIdResponse())
            .ToListAsync(cancellationToken);
    }

    public virtual async Task DeleteAsync(DeleteProductCategoryCommand command, Guid companyId, CancellationToken cancellationToken = default)
    {
        await _productCategoryRepository.DeleteAsync(command.Id, cancellationToken);
    }
}
