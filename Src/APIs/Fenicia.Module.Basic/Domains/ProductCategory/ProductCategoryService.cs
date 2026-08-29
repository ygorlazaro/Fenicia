using Fenicia.Common;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.ProductCategory.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.ProductCategory;

public class ProductCategoryService(ProductCategoryRepository productCategoryRepository)
{
    public ProductCategoryService()
        : this(null!)
    {
    }

    public async Task<Pagination<List<GetAllProductCategoryResponse>>> GetAllAsync(GetAllProductCategoryQuery query, CancellationToken ct)
    {
        var total = await productCategoryRepository.CountAsync(ct);

        var categories = await productCategoryRepository.Query()
            .Select(pc => new GetAllProductCategoryResponse(pc.Id, pc.Name))
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(ct);

        return new Pagination<List<GetAllProductCategoryResponse>>(categories, total, query.Page, query.PerPage);
    }

    public async Task<GetProductCategoryByIdResponse?> GetByIdAsync(GetProductCategoryByIdQuery query, CancellationToken ct)
    {
        var category = await productCategoryRepository.GetByIdAsync(query.Id, ct);

        return category switch
        {
            null => null,
            _ => new GetProductCategoryByIdResponse(category.Id, category.Name)
        };
    }

    public async Task<AddProductCategoryResponse> AddAsync(AddProductCategoryCommand command, Guid companyId, CancellationToken ct)
    {
        var category = new ProductCategoryModel
        {
            Id = command.Id,
            Name = command.Name,
            CompanyId = companyId
        };

        await productCategoryRepository.InsertAsync(category, ct);

        return new AddProductCategoryResponse(category.Id, category.Name);
    }

    public async Task<UpdateProductCategoryResponse?> UpdateAsync(UpdateProductCategoryCommand command, Guid companyId, CancellationToken ct)
    {
        var category = await productCategoryRepository.GetByIdAsync(command.Id, ct);

        if (category is null)
        {
            return null;
        }

        category.Name = command.Name;
        category.CompanyId = companyId;

        await productCategoryRepository.UpdateAsync(command.Id, category, ct);

        return new UpdateProductCategoryResponse(category.Id, category.Name);
    }

    public async Task DeleteAsync(DeleteProductCategoryCommand command, Guid companyId, CancellationToken ct)
    {
        var category = await productCategoryRepository.GetByIdAsync(command.Id, ct);

        if (category is null)
        {
            return;
        }

        category.Deleted = DateTime.Now;
        category.CompanyId = companyId;

        await productCategoryRepository.UpdateAsync(command.Id, category, ct);
    }
}
