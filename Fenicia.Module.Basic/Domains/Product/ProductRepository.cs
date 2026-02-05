using Fenicia.Common;
using Fenicia.Common.Database.Abstracts;
using Fenicia.Common.Database.Contexts;
using Fenicia.Common.Database.Models.Basic;
using Fenicia.Common.Exceptions;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Product;

public class ProductRepository(BasicContext context) : BaseRepository<ProductModel>(context), IProductRepository
{
    public async Task<List<ProductModel>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken, int page, int perPage)
    {
        return await context.Products.Where(x => x.CategoryId == categoryId).Skip((page - 1) * perPage).Take(perPage).ToListAsync(cancellationToken);
    }

    public async Task IncreaseStockAsync(Guid productId, double quantity, CancellationToken cancellationToken)
    {
        var product = await GetByIdAsync(productId, cancellationToken);

        if (product is null)
        {
            throw new ItemNotExistsException(TextConstants.ItemNotFoundMessage);
        }

        product.Quantity += quantity;

        Update(product);

        await SaveChangesAsync(cancellationToken);
    }

    public async Task DecreastStockAsync(Guid productId, double quantity, CancellationToken cancellationToken)
    {
        var product = await GetByIdAsync(productId, cancellationToken);

        if (product is null)
        {
            throw new ItemNotExistsException(TextConstants.ItemNotFoundMessage);
        }

        product.Quantity -= quantity;

        Update(product);

        await SaveChangesAsync(cancellationToken);
    }
}
