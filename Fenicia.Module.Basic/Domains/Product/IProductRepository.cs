using Fenicia.Common.Database.Abstracts;
using Fenicia.Common.Database.Models.Basic;

namespace Fenicia.Module.Basic.Domains.Product;

public interface IProductRepository : IBaseRepository<ProductModel>
{
    Task<List<ProductModel>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken, int page, int perPage);

    Task IncreaseStockAsync(Guid productId, double quantity, CancellationToken cancellationToken);

    Task DecreastStockAsync(Guid productId, double quantity, CancellationToken cancellationToken);
}
