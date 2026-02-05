using Fenicia.Common.Database.Converters.Basic;
using Fenicia.Common.Database.Requests.Basic;
using Fenicia.Common.Database.Responses.Basic;

namespace Fenicia.Module.Basic.Domains.ProductCategory;

public class ProductCategoryService(IProductCategoryRepository productCategoryRepository) : IProductCategoryService
{
    public async Task<List<ProductCategoryResponse>> GetAllAsync(CancellationToken cancellationToken, int page = 1, int perPage = 1)
    {
        var productCategories = await productCategoryRepository.GetAllAsync(cancellationToken, page, perPage);

        return ProductCategoryConverter.Convert(productCategories);
    }

    public async Task<ProductCategoryResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var productCategories = await productCategoryRepository.GetByIdAsync(id, cancellationToken);

        return productCategories is null ? null : ProductCategoryConverter.Convert(productCategories);
    }

    public async Task<ProductCategoryResponse?> AddAsync(ProductCategoryRequest request, CancellationToken cancellationToken)
    {
        var productCategories = ProductCategoryConverter.Convert(request);

        productCategoryRepository.Add(productCategories);

        await productCategoryRepository.SaveChangesAsync(cancellationToken);

        return ProductCategoryConverter.Convert(productCategories);
    }

    public async Task<ProductCategoryResponse?> UpdateAsync(ProductCategoryRequest request, CancellationToken cancellationToken)
    {
        var productCategories = ProductCategoryConverter.Convert(request);

        productCategoryRepository.Update(productCategories);

        await productCategoryRepository.SaveChangesAsync(cancellationToken);

        return ProductCategoryConverter.Convert(productCategories);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        productCategoryRepository.Delete(id);

        await productCategoryRepository.SaveChangesAsync(cancellationToken);
    }
}
