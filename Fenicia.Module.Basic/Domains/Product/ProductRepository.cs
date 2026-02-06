using Fenicia.Common;
using Fenicia.Common.Data.Abstracts;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
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

    public async Task<List<ProductModel>> GetInventoryAsync(Guid productId, CancellationToken cancellationToken, int page, int perPage)
    {
        return await context.Products.Where(p => p.Id == productId).OrderBy(p => p.Quantity).Skip((page - 1) * perPage).Take(perPage).Include(p => p.Category).ToListAsync(cancellationToken);
    }

    public async Task<decimal> GetTotalCostPriceByProductAsync(Guid productId, CancellationToken cancellationToken)
    {
        return await context.Products.Where(p => p.Id == productId).SumAsync(p => p.CostPrice ?? 0, cancellationToken);
    }

    public async Task<decimal> GetTotalSalesPriceProductAsync(Guid productId, CancellationToken cancellationToken)
    {
        return await context.Products.Where(p => p.Id == productId).SumAsync(p => p.SalesPrice, cancellationToken);
    }

    public async Task<double> GetTotalQuantityProductAsync(Guid productId, CancellationToken cancellationToken)
    {
        return await context.Products.Where(p => p.Id == productId).SumAsync(p => p.Quantity, cancellationToken);
    }

    public async Task<List<ProductModel>> GetInventoryByCategoryAsync(Guid categoryId, CancellationToken cancellationToken, int page, int perPage)
    {
        return await context.Products.Where(p => p.CategoryId == categoryId).OrderBy(p => p.Quantity).Skip((page - 1) * perPage).Take(perPage).Include(p => p.Category).ToListAsync(cancellationToken);
    }

    public async Task<decimal> GetTotalCostPriceByCategoryAsync(Guid categoryId, CancellationToken cancellationToken)
    {
        return await context.Products.Where(p => p.CategoryId == categoryId).SumAsync(p => p.CostPrice ?? 0, cancellationToken);
    }

    public async Task<decimal> GetTotalSalesPriceCategoryAsync(Guid categoryId, CancellationToken cancellationToken)
    {
        return await context.Products.Where(p => p.CategoryId == categoryId).SumAsync(p => p.SalesPrice, cancellationToken);
    }

    public async Task<double> GetTotalQuantityCategoryAsync(Guid categoryId, CancellationToken cancellationToken)
    {
        return await context.Products.Where(p => p.CategoryId == categoryId).SumAsync(p => p.Quantity, cancellationToken);
    }

    public async Task<List<ProductModel>> GetInventoryAsync(CancellationToken cancellationToken, int page, int perPage)
    {
        return await context.Products.OrderBy(p => p.Quantity).Skip((page - 1) * perPage).Take(perPage).Include(p => p.Category).ToListAsync(cancellationToken);
    }

    public async Task<decimal> GetTotalCostPriceAsync(CancellationToken cancellationToken)
    {
        return await context.Products.SumAsync(p => p.CostPrice ?? 0, cancellationToken);
    }

    public async Task<decimal> GetTotalSalesPriceAsync(CancellationToken cancellationToken)
    {
        return await context.Products.SumAsync(p => p.SalesPrice, cancellationToken);
    }

    public async Task<double> GetTotalQuantityAsync(CancellationToken cancellationToken)
    {
        return await context.Products.SumAsync(p => p.Quantity, cancellationToken);
    }
}
