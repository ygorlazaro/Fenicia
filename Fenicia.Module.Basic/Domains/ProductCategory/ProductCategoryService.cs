using Fenicia.Common.Data.Mappers.Basic;
using Fenicia.Common.Data.Requests.Basic;
using Fenicia.Common.Data.Responses.Basic;

namespace Fenicia.Module.Basic.Domains.ProductCategory;

public class ProductCategoryService(IProductCategoryRepository productCategoryRepository) : IProductCategoryService
{
    public async Task<List<ProductCategoryResponse>> GetAllAsync(CancellationToken ct, int page = 1, int perPage = 1)
    {
        var productCategories = await productCategoryRepository.GetAllAsync(ct, page, perPage);

        return ProductCategoryMapper.Map(productCategories);
    }

    public async Task<ProductCategoryResponse?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var productCategories = await productCategoryRepository.GetByIdAsync(id, ct);

        return productCategories is null ? null : ProductCategoryMapper.Map(productCategories);
    }

    public async Task<ProductCategoryResponse?> AddAsync(ProductCategoryRequest request, CancellationToken ct)
    {
        var productCategories = ProductCategoryMapper.Map(request);

        productCategoryRepository.Add(productCategories);

        await productCategoryRepository.SaveChangesAsync(ct);

        return ProductCategoryMapper.Map(productCategories);
    }

    public async Task<ProductCategoryResponse?> UpdateAsync(ProductCategoryRequest request, CancellationToken ct)
    {
        var productCategories = ProductCategoryMapper.Map(request);

        productCategoryRepository.Update(productCategories);

        await productCategoryRepository.SaveChangesAsync(ct);

        return ProductCategoryMapper.Map(productCategories);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        productCategoryRepository.Delete(id);

        await productCategoryRepository.SaveChangesAsync(ct);
    }
}
