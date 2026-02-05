using Fenicia.Common.Data.Requests.Basic;
using Fenicia.Common.Data.Responses.Basic;

namespace Fenicia.Module.Basic.Domains.Product;

public interface IProductService
{
    Task<List<ProductResponse>> GetAllAsync(CancellationToken cancellationToken, int page = 1, int perPage = 1);

    Task<ProductResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<ProductResponse?> AddAsync(ProductRequest request, CancellationToken cancellationToken);

    Task<ProductResponse?> UpdateAsync(ProductRequest request, CancellationToken cancellationToken);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken);

    Task<List<ProductResponse>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken, int page, int perPage);
}
