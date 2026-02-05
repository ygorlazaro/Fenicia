using Fenicia.Common.Database.Requests.Basic;
using Fenicia.Common.Database.Responses.Basic;

namespace Fenicia.Module.Basic.Domains.ProductCategory;

public interface IProductCategoryService
{
    Task<List<ProductCategoryResponse>> GetAllAsync(CancellationToken cancellationToken, int page = 1, int perPage = 1);

    Task<ProductCategoryResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<ProductCategoryResponse?> AddAsync(ProductCategoryRequest request, CancellationToken cancellationToken);

    Task<ProductCategoryResponse?> UpdateAsync(ProductCategoryRequest request, CancellationToken cancellationToken);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
