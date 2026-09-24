using Fenicia.Common;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.DTOs.Basic.ProductCategory;
using Fenicia.Module.Basic.Domains.ProductCategory.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.ProductCategory;

public sealed class ProductCategoryService(IProductCategoryRepository repository) : IProductCategoryService
{
    public async Task<Pagination<List<GetAllProductCategoryResponse>>> GetAllAsync(
        int page = 1,
        int perPage = 10,
        CancellationToken cancellationToken = default)
    {
        var baseQuery = repository.Query()
            .Where(pc => pc.Deleted == null);

        var total = await baseQuery.CountAsync(cancellationToken);

        var categories = await baseQuery
            .Select(pc => new GetAllProductCategoryResponse(pc.Id, pc.Name))
            .Skip(page * perPage)
            .Take(perPage)
            .ToListAsync(cancellationToken);

        return new Pagination<List<GetAllProductCategoryResponse>>(categories, total, page, perPage);
    }

    public async Task<GetProductCategoryByIdResponse?> GetByIdAsync(
        GetProductCategoryByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var category = await repository.GetByIdAsync(query.Id, cancellationToken);

        return category is null ? null : new GetProductCategoryByIdResponse(category.Id, category.Name);
    }

    public async Task<AddProductCategoryResponse> AddAsync(
        AddProductCategoryRequest command,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        var category = new ProductCategoryModel
        {
            Id = command.Id,
            Name = command.Name,
            CompanyId = companyId
        };

        await repository.InsertAsync(category, cancellationToken);

        return new AddProductCategoryResponse(category.Id, category.Name);
    }

    public async Task<UpdateProductCategoryResponse?> UpdateAsync(
        UpdateProductCategoryRequest command,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        var category = await repository.GetByIdAsync(command.Id, cancellationToken);

        if (category is null)
        {
            return null;
        }

        category.Name = command.Name;

        await repository.UpdateAsync(command.Id, category, cancellationToken);

        return new UpdateProductCategoryResponse(category.Id, category.Name);
    }

    public Task<List<GetProductCategoryByIdResponse>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken = default)
    {
        var idList = ids.ToList();
        return repository.Query()
            .Where(pc => idList.Contains(pc.Id))
            .Select(pc => new GetProductCategoryByIdResponse(pc.Id, pc.Name))
            .ToListAsync(cancellationToken);
    }

    public async Task DeleteAsync(
        DeleteProductCategoryRequest command,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        await repository.DeleteAsync(command.Id, cancellationToken);
    }
}
