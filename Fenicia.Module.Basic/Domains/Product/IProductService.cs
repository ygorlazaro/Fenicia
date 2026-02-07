using Fenicia.Common.Data.Requests.Basic;
using Fenicia.Common.Data.Responses.Basic;

namespace Fenicia.Module.Basic.Domains.Product;

public interface IProductService
{
    Task<List<ProductResponse>> GetAllAsync(CancellationToken ct, int page = 1, int perPage = 1);

    Task<ProductResponse?> GetByIdAsync(Guid id, CancellationToken ct);

    Task<ProductResponse?> AddAsync(ProductRequest request, CancellationToken ct);

    Task<ProductResponse?> UpdateAsync(ProductRequest request, CancellationToken ct);

    Task DeleteAsync(Guid id, CancellationToken ct);

    Task<List<ProductResponse>> GetByCategoryIdAsync(Guid categoryId, CancellationToken ct, int page, int perPage);
}