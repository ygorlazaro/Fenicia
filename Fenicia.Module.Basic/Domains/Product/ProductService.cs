using Fenicia.Common.Data.Converters.Basic;
using Fenicia.Common.Data.Requests.Basic;
using Fenicia.Common.Data.Responses.Basic;

namespace Fenicia.Module.Basic.Domains.Product;

public class ProductService(IProductRepository productRepository) : IProductService
{
    public async Task<List<ProductResponse>> GetAllAsync(CancellationToken cancellationToken, int page = 1, int perPage = 1)
    {
        var products = await productRepository.GetAllAsync(cancellationToken, page, perPage);

        return ProductConverter.Convert(products);
    }

    public async Task<ProductResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(id, cancellationToken);

        return product is null ? null : ProductConverter.Convert(product);
    }

    public async Task<ProductResponse?> AddAsync(ProductRequest request, CancellationToken cancellationToken)
    {
        var product = ProductConverter.Convert(request);

        productRepository.Add(product);

        await productRepository.SaveChangesAsync(cancellationToken);

        return ProductConverter.Convert(product);
    }

    public async Task<ProductResponse?> UpdateAsync(ProductRequest request, CancellationToken cancellationToken)
    {
        var product = ProductConverter.Convert(request);

        productRepository.Update(product);

        await productRepository.SaveChangesAsync(cancellationToken);

        return ProductConverter.Convert(product);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        productRepository.Delete(id);

        await productRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<ProductResponse>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken, int page, int perPage)
    {
        var products = await productRepository.GetByCategoryIdAsync(categoryId, cancellationToken, page, perPage);

        return ProductConverter.Convert(products);
    }
}
