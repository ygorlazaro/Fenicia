using Fenicia.Common;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.DTOs.Basic.ProductCategory;
using Fenicia.Module.Basic.Domains.ProductCategory.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.ProductCategory;

public sealed class ProductCategoryService(IProductCategoryRepository productCategoryRepository, ProductCategoryMapper productCategoryMapper) : IProductCategoryService
{
    public async Task<Pagination<List<GetAllProductCategoryResponse>>> GetAllAsync(
        int page = 1,
        int perPage = 10,
        CancellationToken cancellationToken = default)
    {
        var baseQuery = productCategoryRepository.Query()
            .Where(pc => pc.Deleted == null);

        var total = await baseQuery.CountAsync(cancellationToken);

        var categories = await baseQuery
            .Select(pc => productCategoryMapper.MapToGetAllProductCategoryResponse(pc))
            .Skip(page * perPage)
            .Take(perPage)
            .ToListAsync(cancellationToken);

        return new Pagination<List<GetAllProductCategoryResponse>>(categories, total, page, perPage);
    }

    public async Task<GetProductCategoryByIdResponse?> GetByIdAsync(
        GetProductCategoryByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var category = await productCategoryRepository.GetByIdAsync(query.Id, cancellationToken);

        return category is null ? null : productCategoryMapper.MapToGetProductCategoryByIdResponse(category);
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

        return productCategoryMapper.MapToAddProductCategoryResponse(category);
    }

    public async Task<UpdateProductCategoryResponse?> UpdateAsync(
        UpdateProductCategoryCommand command,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        var category = await productCategoryRepository.GetByIdAsync(command.Id, cancellationToken);

        Console.WriteLine(category?.Id);

        if (category is null)
        {
            return null;
        }

        category.Name = command.Name;

        await productCategoryRepository.UpdateAsync(command.Id, category, cancellationToken);

        return productCategoryMapper.MapToUpdateProductCategoryResponse(category);
    }

    public Task<List<GetProductCategoryByIdResponse>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken = default)
    {
        var idList = ids.ToList();
        return productCategoryRepository.Query()
            .Where(pc => idList.Contains(pc.Id))
            .Select(pc => productCategoryMapper.MapToGetProductCategoryByIdResponse(pc))
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
