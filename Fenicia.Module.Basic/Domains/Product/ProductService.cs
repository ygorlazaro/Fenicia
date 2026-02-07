using Fenicia.Common.Data.Mappers.Basic;
using Fenicia.Common.Data.Requests.Basic;
using Fenicia.Common.Data.Responses.Basic;

namespace Fenicia.Module.Basic.Domains.Product;

public class ProductService(IProductRepository productRepository) : IProductService
{
    public async Task<List<ProductResponse>> GetAllAsync(CancellationToken ct, int page = 1, int perPage = 1)
    {
        var products = await productRepository.GetAllAsync(ct, page, perPage);

        return ProductMapper.Map(products);
    }

    public async Task<ProductResponse?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var product = await productRepository.GetByIdAsync(id, ct);

        return product is null ? null : ProductMapper.Map(product);
    }

    public async Task<ProductResponse?> AddAsync(ProductRequest request, CancellationToken ct)
    {
        var product = ProductMapper.Map(request);

        productRepository.Add(product);

        await productRepository.SaveChangesAsync(ct);

        return ProductMapper.Map(product);
    }

    public async Task<ProductResponse?> UpdateAsync(ProductRequest request, CancellationToken ct)
    {
        var product = ProductMapper.Map(request);

        productRepository.Update(product);

        await productRepository.SaveChangesAsync(ct);

        return ProductMapper.Map(product);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        productRepository.Delete(id);

        await productRepository.SaveChangesAsync(ct);
    }

    public async Task<List<ProductResponse>> GetByCategoryIdAsync(Guid categoryId, CancellationToken ct, int page, int perPage)
    {
        var products = await productRepository.GetByCategoryIdAsync(categoryId, ct, page, perPage);

        return ProductMapper.Map(products);
    }
}
