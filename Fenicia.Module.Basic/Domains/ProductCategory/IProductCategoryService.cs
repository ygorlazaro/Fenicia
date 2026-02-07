using Fenicia.Common.Data.Requests.Basic;
using Fenicia.Common.Data.Responses.Basic;

namespace Fenicia.Module.Basic.Domains.ProductCategory;

public interface IProductCategoryService
{
    Task<List<ProductCategoryResponse>> GetAllAsync(CancellationToken ct, int page = 1, int perPage = 1);

    Task<ProductCategoryResponse?> GetByIdAsync(Guid id, CancellationToken ct);

    Task<ProductCategoryResponse?> AddAsync(ProductCategoryRequest request, CancellationToken ct);

    Task<ProductCategoryResponse?> UpdateAsync(ProductCategoryRequest request, CancellationToken ct);

    Task DeleteAsync(Guid id, CancellationToken ct);
}
