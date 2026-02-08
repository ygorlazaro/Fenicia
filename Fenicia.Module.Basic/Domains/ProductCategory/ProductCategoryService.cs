using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Requests.Basic;
using Fenicia.Common.Data.Responses.Basic;

namespace Fenicia.Module.Basic.Domains.ProductCategory;

public class ProductCategoryService(IProductCategoryRepository productCategoryRepository) : IProductCategoryService
{
    public async Task<List<ProductCategoryResponse>> GetAllAsync(CancellationToken ct, int page = 1, int perPage = 1)
    {
        var productCategories = await productCategoryRepository.GetAllAsync(ct, page, perPage);

        return [..productCategories.Select(pc => new ProductCategoryResponse(pc))];
    }

    public async Task<ProductCategoryResponse?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var productCategory = await productCategoryRepository.GetByIdAsync(id, ct);

        return productCategory is null ? null : new ProductCategoryResponse(productCategory);
    }

    public async Task<ProductCategoryResponse?> AddAsync(ProductCategoryRequest request, CancellationToken ct)
    {
        var productCategory = new ProductCategoryModel(request);

        productCategoryRepository.Add(productCategory);

        await productCategoryRepository.SaveChangesAsync(ct);

        return new ProductCategoryResponse(productCategory);
    }

    public async Task<ProductCategoryResponse?> UpdateAsync(ProductCategoryRequest request, CancellationToken ct)
    {
        var productCategory = new ProductCategoryModel(request);

        productCategoryRepository.Update(productCategory);

        await productCategoryRepository.SaveChangesAsync(ct);

        return new ProductCategoryResponse(productCategory);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        await productCategoryRepository.DeleteAsync(id, ct);

        await productCategoryRepository.SaveChangesAsync(ct);
    }
}