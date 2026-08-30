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

    public virtual async Task<Pagination<List<GetAllProductCategoryResponse>>> GetAllAsync(GetAllProductCategoryQuery query, CancellationToken ct)
    {
        var total = await _productCategoryRepository.CountAsync(ct);

        var categories = await _productCategoryRepository.Query()
            .Select(pc => pc.MapToGetAllProductCategoryResponse())
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(ct);

        return new Pagination<List<GetAllProductCategoryResponse>>(categories, total, query.Page, query.PerPage);
    }

    public virtual async Task<GetProductCategoryByIdResponse?> GetByIdAsync(GetProductCategoryByIdQuery query, CancellationToken ct)
    {
        var category = await _productCategoryRepository.GetByIdAsync(query.Id, ct);

        return category is null ? null : category.MapToGetProductCategoryByIdResponse();
    }

    public virtual async Task<AddProductCategoryResponse> AddAsync(AddProductCategoryCommand command, Guid companyId, CancellationToken ct)
    {
        var category = new ProductCategoryModel
        {
            Id = command.Id,
            Name = command.Name,
            CompanyId = companyId
        };

        await _productCategoryRepository.InsertAsync(category, ct);

        return category.MapToAddProductCategoryResponse();
    }

    public virtual async Task<UpdateProductCategoryResponse?> UpdateAsync(UpdateProductCategoryCommand command, Guid companyId, CancellationToken ct)
    {
        var category = await _productCategoryRepository.GetByIdAsync(command.Id, ct);

        if (category is null)
        {
            return null;
        }

        category.Name = command.Name;
        category.CompanyId = companyId;

        await _productCategoryRepository.UpdateAsync(command.Id, category, ct);

        return category.MapToUpdateProductCategoryResponse();
    }

    public virtual async Task<List<GetProductCategoryByIdResponse>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default)
    {
        var idList = ids.ToList();
        return await _productCategoryRepository.Query()
            .Where(pc => idList.Contains(pc.Id))
            .Select(pc => pc.MapToGetProductCategoryByIdResponse())
            .ToListAsync(ct);
    }

    public virtual async Task DeleteAsync(DeleteProductCategoryCommand command, Guid companyId, CancellationToken ct)
    {
        await _productCategoryRepository.DeleteAsync(command.Id, ct);
    }
}
