using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Product;

public class ProductRepository(DefaultContext context) : Repository<ProductModel>(context), IProductRepository
{
    public async Task<IEnumerable<ProductModel>> GetAllWithDetailsAsync(int page = 1, int perPage = 10, CancellationToken ct)
    {
        return await DbSet
                .Include(p => p.Category)
            .Include(p => p.Supplier).ThenInclude(s => s != null ? s.Person : null)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync(ct);
    }

    public async Task<ProductModel?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct)
    {
        return await DbSet
                .Include(p => p.Category)
            .Include(p => p.Supplier).ThenInclude(s => s != null ? s.Person : null)
            .FirstOrDefaultAsync(e => e.Id == id, ct);
    }

    public async Task<IEnumerable<ProductModel>> GetByCategoryIdAsync(Guid categoryId, int page = 1, int perPage = 10, CancellationToken ct)
    {
        return await DbSet
                .Where(p => p.CategoryId == categoryId)
            .Include(p => p.Category)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<ProductModel>> GetAllWithCategoryAsync(int page = 1, int perPage = 10, CancellationToken ct)
    {
        return await DbSet
                .Include(p => p.Category)
            .OrderBy(p => p.Quantity)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<ProductModel>> GetByCategoryWithCategoryAsync(Guid categoryId, int page = 1, int perPage = 10, CancellationToken ct)
    {
        return await DbSet
                .Where(p => p.CategoryId == categoryId)
            .Include(p => p.Category)
            .OrderBy(p => p.Quantity)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<ProductModel>> GetByIdWithCategoryAsync(Guid productId, int page = 1, int perPage = 10, CancellationToken ct)
    {
        return await DbSet
                .Where(p => p.Id == productId)
            .Include(p => p.Category)
            .OrderBy(p => p.Quantity)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync(ct);
    }

    public async Task<List<ProductModel>> GetLowStockAsync(CancellationToken ct)
    {
        return await DbSet
                .Include(p => p.Category)
            .OrderBy(p => p.Quantity)
            .Take(5)
            .ToListAsync(ct);
    }

    public async Task<decimal> GetTotalCostPriceAsync(CancellationToken ct)
    {
        return await DbSet.SumAsync(p => p.CostPrice ?? 0, ct);
    }

    public async Task<decimal> GetTotalSalesPriceAsync(CancellationToken ct)
    {
        return await DbSet.SumAsync(p => p.SalesPrice, ct);
    }

    public async Task<int> GetTotalQuantityAsync(CancellationToken ct)
    {
        return (int)await DbSet.SumAsync(p => (double)p.Quantity, ct);
    }

    public async Task<decimal> GetTotalCostPriceByCategoryAsync(Guid categoryId, CancellationToken ct)
    {
        return await DbSet.Where(p => p.CategoryId == categoryId).SumAsync(p => p.CostPrice ?? 0, ct);
    }

    public async Task<decimal> GetTotalSalesPriceByCategoryAsync(Guid categoryId, CancellationToken ct)
    {
        return await DbSet.Where(p => p.CategoryId == categoryId).SumAsync(p => p.SalesPrice, ct);
    }

    public async Task<int> GetTotalQuantityByCategoryAsync(Guid categoryId, CancellationToken ct)
    {
        return (int)await DbSet.Where(p => p.CategoryId == categoryId).SumAsync(p => (double)p.Quantity, ct);
    }

    public async Task<decimal> GetTotalCostPriceByProductAsync(Guid productId, CancellationToken ct)
    {
        return await DbSet.Where(p => p.Id == productId).SumAsync(p => p.CostPrice ?? 0, ct);
    }

    public async Task<decimal> GetTotalSalesPriceByProductAsync(Guid productId, CancellationToken ct)
    {
        return await DbSet.Where(p => p.Id == productId).SumAsync(p => p.SalesPrice, ct);
    }

    public async Task<int> GetTotalQuantityByProductAsync(Guid productId, CancellationToken ct)
    {
        return (int)await DbSet.Where(p => p.Id == productId).SumAsync(p => (double)p.Quantity, ct);
    }

    public async Task<decimal> GetTotalCostValueAsync(CancellationToken ct)
    {
        return await DbSet.SumAsync(p => (p.CostPrice ?? 0) * (decimal)p.Quantity, ct);
    }

    public async Task<decimal> GetTotalSalesValueAsync(CancellationToken ct)
    {
        return await DbSet.SumAsync(p => p.SalesPrice * (decimal)p.Quantity, ct);
    }

    public async Task<List<ProductModel>> GetZeroMovementCandidatesAsync(IEnumerable<Guid> activeProductIds, CancellationToken ct)
    {
        var activeIds = activeProductIds as HashSet<Guid> ?? [.. activeProductIds];
        return await DbSet
            .Where(p => p.Quantity > 0 && !activeIds.Contains(p.Id))
            .Include(p => p.Category)
            .Include(p => p.Supplier)
                .ThenInclude(s => s!.Person)
            .ToListAsync(ct);
    }

    public async Task<List<ProductModel>> GetOverstockCandidatesAsync(CancellationToken ct)
    {
        return await DbSet
                .Where(p => p.Quantity > 0)
            .Include(p => p.Category)
            .Include(p => p.Supplier)
                .ThenInclude(s => s!.Person)
            .ToListAsync(ct);
    }

    public async Task<List<(Guid CategoryId, string CategoryName, int Quantity, decimal? CostPrice)>> GetStockValueByCategoryAsync(CancellationToken ct)
    {
        return await DbSet
                .Where(p => p.Quantity > 0)
            .Select(p => new { p.CategoryId, CategoryName = p.Category.Name, p.Quantity, p.CostPrice })
            .ToListAsync(ct)
            .ContinueWith(t => t.Result.GroupBy(p => new { p.CategoryId, p.CategoryName }).Select(g => (g.Key.CategoryId, g.Key.CategoryName, g.Count(), g.First().CostPrice)).OrderByDescending(g => g.CostPrice * (decimal)g.Item3).ToList(), ct);
    }
}
