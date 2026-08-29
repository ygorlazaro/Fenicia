using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Repositories;
using Fenicia.Module.Basic.Domains.Inventory.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Inventory;

public class ProductRepository(DefaultContext context) : Repository<ProductModel>(context)
{
    public async Task<IEnumerable<ProductModel>> GetAllWithCategoryAsync(int page = 1, int perPage = 10, CancellationToken ct = default)
    {
        return await DbSet
                .Include(p => p.Category)
            .Where(p => p.Deleted == null)
            .OrderBy(p => p.Quantity)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<ProductModel>> GetByCategoryWithCategoryAsync(Guid categoryId, int page = 1, int perPage = 10, CancellationToken ct = default)
    {
        return await DbSet
                .Where(p => p.CategoryId == categoryId && p.Deleted == null)
            .Include(p => p.Category)
            .OrderBy(p => p.Quantity)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<ProductModel>> GetByIdWithCategoryAsync(Guid productId, int page = 1, int perPage = 10, CancellationToken ct = default)
    {
        return await DbSet
                .Where(p => p.Id == productId && p.Deleted == null)
            .Include(p => p.Category)
            .OrderBy(p => p.Quantity)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync(ct);
    }

    public async Task<List<ProductModel>> GetLowStockAsync(CancellationToken ct = default)
    {
        return await DbSet
                .Where(p => p.Deleted == null)
            .Include(p => p.Category)
            .OrderBy(p => p.Quantity)
            .Take(5)
            .ToListAsync(ct);
    }

    public async Task<decimal> GetTotalCostPriceAsync(CancellationToken ct = default)
    {
        return await DbSet.SumAsync(p => p.CostPrice ?? 0, ct);
    }

    public async Task<decimal> GetTotalSalesPriceAsync(CancellationToken ct = default)
    {
        return await DbSet.SumAsync(p => p.SalesPrice, ct);
    }

    public async Task<int> GetTotalQuantityAsync(CancellationToken ct = default)
    {
        return (int)await DbSet.SumAsync(p => (double)p.Quantity, ct);
    }

    public async Task<decimal> GetTotalCostPriceByCategoryAsync(Guid categoryId, CancellationToken ct = default)
    {
        return await DbSet.Where(p => p.CategoryId == categoryId).SumAsync(p => p.CostPrice ?? 0, ct);
    }

    public async Task<decimal> GetTotalSalesPriceByCategoryAsync(Guid categoryId, CancellationToken ct = default)
    {
        return await DbSet.Where(p => p.CategoryId == categoryId).SumAsync(p => p.SalesPrice, ct);
    }

    public async Task<int> GetTotalQuantityByCategoryAsync(Guid categoryId, CancellationToken ct = default)
    {
        return (int)await DbSet.Where(p => p.CategoryId == categoryId).SumAsync(p => (double)p.Quantity, ct);
    }

    public async Task<decimal> GetTotalCostPriceByProductAsync(Guid productId, CancellationToken ct = default)
    {
        return await DbSet.Where(p => p.Id == productId).SumAsync(p => p.CostPrice ?? 0, ct);
    }

    public async Task<decimal> GetTotalSalesPriceByProductAsync(Guid productId, CancellationToken ct = default)
    {
        return await DbSet.Where(p => p.Id == productId).SumAsync(p => p.SalesPrice, ct);
    }

    public async Task<int> GetTotalQuantityByProductAsync(Guid productId, CancellationToken ct = default)
    {
        return (int)await DbSet.Where(p => p.Id == productId).SumAsync(p => (double)p.Quantity, ct);
    }

    public async Task<decimal> GetTotalCostValueAsync(CancellationToken ct = default)
    {
        return await DbSet.SumAsync(p => (p.CostPrice ?? 0) * (decimal)p.Quantity, ct);
    }

    public async Task<decimal> GetTotalSalesValueAsync(CancellationToken ct = default)
    {
        return await DbSet.SumAsync(p => p.SalesPrice * (decimal)p.Quantity, ct);
    }

    public async Task<List<CategoryBreakdownResponse>> GetCategoryBreakdownAsync(CancellationToken ct = default)
    {
        return await DbSet
                .GroupBy(p => new { p.CategoryId, CategoryName = p.Category.Name })
            .Select(g => new CategoryBreakdownResponse(
                g.Key.CategoryId,
                g.Key.CategoryName,
                g.Sum(p => (decimal)(p.CostPrice ?? 0) * (decimal)p.Quantity),
                g.Sum(p => p.SalesPrice * (decimal)p.Quantity),
                g.Sum(p => p.Quantity)))
            .ToListAsync(ct);
    }

    public async Task<List<ProductModel>> GetZeroMovementCandidatesAsync(IEnumerable<Guid> activeProductIds, CancellationToken ct = default)
    {
        var activeIds = activeProductIds as HashSet<Guid> ?? [.. activeProductIds];
        return await DbSet
            .Where(p => p.Quantity > 0 && !activeIds.Contains(p.Id) && p.Deleted == null)
            .Include(p => p.Category)
            .Include(p => p.Supplier)
                .ThenInclude(s => s!.Person)
            .ToListAsync(ct);
    }

    public async Task<List<ProductModel>> GetOverstockCandidatesAsync(CancellationToken ct = default)
    {
        return await DbSet
                .Where(p => p.Quantity > 0 && p.Deleted == null)
            .Include(p => p.Category)
            .Include(p => p.Supplier)
                .ThenInclude(s => s!.Person)
            .ToListAsync(ct);
    }

    public async Task<List<(Guid CategoryId, string CategoryName, int Quantity, decimal? CostPrice)>> GetStockValueByCategoryAsync(CancellationToken ct = default)
    {
        return await DbSet
                .Where(p => p.Quantity > 0 && p.Deleted == null)
            .Select(p => new { p.CategoryId, CategoryName = p.Category.Name, p.Quantity, p.CostPrice })
            .ToListAsync(ct)
            .ContinueWith(t => t.Result.GroupBy(p => new { p.CategoryId, p.CategoryName }).Select(g => (g.Key.CategoryId, g.Key.CategoryName, g.Count(), g.First().CostPrice)).OrderByDescending(g => g.CostPrice * (decimal)g.Item3).ToList(), ct);
    }
}
