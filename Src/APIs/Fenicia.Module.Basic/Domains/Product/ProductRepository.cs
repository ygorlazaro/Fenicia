using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Product;

public class ProductRepository(DefaultContext context) : Repository<ProductModel>(context), IProductRepository
{
    public async Task<IEnumerable<ProductModel>> GetAllWithDetailsAsync(
        int page = 1,
        int perPage = 10,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(p => p.Category)
            .Include(p => p.Supplier).ThenInclude(s => s != null ? s.Person : null)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync(cancellationToken);
    }

    public Task<ProductModel?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return DbSet
            .Include(p => p.Category)
            .Include(p => p.Supplier).ThenInclude(s => s != null ? s.Person : null)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<ProductModel>> GetByCategoryIdAsync(
        Guid categoryId,
        int page = 1,
        int perPage = 10,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(p => p.CategoryId == categoryId)
            .Include(p => p.Category)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProductModel>> GetAllWithCategoryAsync(
        int page = 1,
        int perPage = 10,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(p => p.Category)
            .OrderBy(p => p.Quantity)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProductModel>> GetByCategoryWithCategoryAsync(
        Guid categoryId,
        int page = 1,
        int perPage = 10,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(p => p.CategoryId == categoryId)
            .Include(p => p.Category)
            .OrderBy(p => p.Quantity)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProductModel>> GetByIdWithCategoryAsync(
        Guid productId,
        int page = 1,
        int perPage = 10,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(p => p.Id == productId)
            .Include(p => p.Category)
            .OrderBy(p => p.Quantity)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync(cancellationToken);
    }

    public Task<List<ProductModel>> GetLowStockAsync(CancellationToken cancellationToken = default)
    {
        return DbSet
            .Include(p => p.Category)
            .OrderBy(p => p.Quantity)
            .Take(5)
            .ToListAsync(cancellationToken);
    }

    public Task<decimal> GetTotalCostPriceAsync(CancellationToken cancellationToken = default)
    {
        return DbSet.SumAsync(p => p.CostPrice ?? 0, cancellationToken);
    }

    public Task<decimal> GetTotalSalesPriceAsync(CancellationToken cancellationToken = default)
    {
        return DbSet.SumAsync(p => p.SalesPrice, cancellationToken);
    }

    public async Task<int> GetTotalQuantityAsync(CancellationToken cancellationToken = default)
    {
        return (int)await DbSet.SumAsync(p => p.Quantity, cancellationToken);
    }

    public Task<decimal> GetTotalCostPriceByCategoryAsync(
        Guid categoryId,
        CancellationToken cancellationToken = default)
    {
        return DbSet.Where(p => p.CategoryId == categoryId).SumAsync(p => p.CostPrice ?? 0, cancellationToken);
    }

    public Task<decimal> GetTotalSalesPriceByCategoryAsync(
        Guid categoryId,
        CancellationToken cancellationToken = default)
    {
        return DbSet.Where(p => p.CategoryId == categoryId).SumAsync(p => p.SalesPrice, cancellationToken);
    }

    public async Task<int> GetTotalQuantityByCategoryAsync(
        Guid categoryId,
        CancellationToken cancellationToken = default)
    {
        return (int)await DbSet.Where(p => p.CategoryId == categoryId).SumAsync(p => p.Quantity, cancellationToken);
    }

    public Task<decimal> GetTotalCostPriceByProductAsync(
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        return DbSet.Where(p => p.Id == productId).SumAsync(p => p.CostPrice ?? 0, cancellationToken);
    }

    public Task<decimal> GetTotalSalesPriceByProductAsync(
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        return DbSet.Where(p => p.Id == productId).SumAsync(p => p.SalesPrice, cancellationToken);
    }

    public async Task<int> GetTotalQuantityByProductAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return (int)await DbSet.Where(p => p.Id == productId).SumAsync(p => p.Quantity, cancellationToken);
    }

    public Task<decimal> GetTotalCostValueAsync(CancellationToken cancellationToken = default)
    {
        return DbSet.SumAsync(p => (p.CostPrice ?? 0) * (decimal)p.Quantity, cancellationToken);
    }

    public Task<decimal> GetTotalSalesValueAsync(CancellationToken cancellationToken = default)
    {
        return DbSet.SumAsync(p => p.SalesPrice * (decimal)p.Quantity, cancellationToken);
    }

    public Task<List<ProductModel>> GetZeroMovementCandidatesAsync(
        IEnumerable<Guid> activeProductIds,
        CancellationToken cancellationToken = default)
    {
        var activeIds = activeProductIds as HashSet<Guid> ?? [.. activeProductIds];
        return DbSet
            .Where(p => p.Quantity > 0 && !activeIds.Contains(p.Id))
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .ThenInclude(s => s!.Person)
            .ToListAsync(cancellationToken);
    }

    public Task<List<ProductModel>> GetOverstockCandidatesAsync(CancellationToken cancellationToken = default)
    {
        return DbSet
            .Where(p => p.Quantity > 0)
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .ThenInclude(s => s!.Person)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<(Guid CategoryId, string CategoryName, int Quantity, decimal? CostPrice)>>
        GetStockValueByCategoryAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(p => p.Quantity > 0)
            .Select(p => new { p.CategoryId, CategoryName = p.Category.Name, p.Quantity, p.CostPrice })
            .ToListAsync(cancellationToken)
            .ContinueWith(
                t => t.Result.GroupBy(p => new { p.CategoryId, p.CategoryName })
                    .Select(g => (g.Key.CategoryId, g.Key.CategoryName, g.Count(), g.First().CostPrice))
                    .OrderByDescending(g => g.CostPrice * g.Item3).ToList(),
                cancellationToken);
    }
}