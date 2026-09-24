using Fenicia.Common;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Repositories;
using Fenicia.Common.DTOs.Basic.Inventory;
using Fenicia.Module.Basic.Domains.Product.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Product;

/// <summary>
/// Repository implementation for managing product data.
/// </summary>
/// <param name="context">The database context.</param>
public class ProductRepository(DbContext context)
    : Repository<ProductModel>(context), IProductRepository
{
    /// <inheritdoc />
    public async Task<IEnumerable<ProductModel>> GetAllWithDetailsAsync(
        PaginationQuery query,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(p => p.Category)
            .Include(p => p.Supplier).ThenInclude(s => s != null ? s.Person : null)
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<ProductModel?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return DbSet
            .Include(p => p.Category)
            .Include(p => p.Supplier).ThenInclude(s => s != null ? s.Person : null)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ProductModel>> GetByCategoryIdAsync(
        Guid categoryId,
        PaginationQuery query,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(p => p.CategoryId == categoryId)
            .Include(p => p.Category)
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ProductModel>> GetAllWithCategoryAsync(
        PaginationQuery query,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(p => p.Category)
            .OrderBy(p => p.Quantity)
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ProductModel>> GetByCategoryWithCategoryAsync(
        Guid categoryId,
        PaginationQuery query,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(p => p.CategoryId == categoryId)
            .Include(p => p.Category)
            .OrderBy(p => p.Quantity)
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ProductModel>> GetByIdWithCategoryAsync(
        Guid productId,
        PaginationQuery query,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(p => p.Id == productId)
            .Include(p => p.Category)
            .OrderBy(p => p.Quantity)
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<List<ProductModel>> GetLowStockAsync(CancellationToken cancellationToken = default)
    {
        return DbSet
            .Include(p => p.Category)
            .OrderBy(p => p.Quantity)
            .Take(5)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<decimal> GetTotalCostPriceAsync(CancellationToken cancellationToken = default)
    {
        return DbSet.SumAsync(p => p.CostPrice ?? 0, cancellationToken);
    }

    /// <inheritdoc />
    public Task<decimal> GetTotalSalesPriceAsync(CancellationToken cancellationToken = default)
    {
        return DbSet.SumAsync(p => p.SalesPrice, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<int> GetTotalQuantityAsync(CancellationToken cancellationToken = default)
    {
        return (int)await DbSet.SumAsync(p => p.Quantity, cancellationToken);
    }

    /// <inheritdoc />
    public Task<decimal> GetTotalCostPriceByCategoryAsync(
        Guid categoryId,
        CancellationToken cancellationToken = default)
    {
        return DbSet.Where(p => p.CategoryId == categoryId).SumAsync(p => p.CostPrice ?? 0, cancellationToken);
    }

    /// <inheritdoc />
    public Task<decimal> GetTotalSalesPriceByCategoryAsync(
        Guid categoryId,
        CancellationToken cancellationToken = default)
    {
        return DbSet.Where(p => p.CategoryId == categoryId).SumAsync(p => p.SalesPrice, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<int> GetTotalQuantityByCategoryAsync(
        Guid categoryId,
        CancellationToken cancellationToken = default)
    {
        return (int)await DbSet.Where(p => p.CategoryId == categoryId).SumAsync(p => p.Quantity, cancellationToken);
    }

    /// <inheritdoc />
    public Task<decimal> GetTotalCostPriceByProductAsync(
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        return DbSet.Where(p => p.Id == productId).SumAsync(p => p.CostPrice ?? 0, cancellationToken);
    }

    /// <inheritdoc />
    public Task<decimal> GetTotalSalesPriceByProductAsync(
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        return DbSet.Where(p => p.Id == productId).SumAsync(p => p.SalesPrice, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<int> GetTotalQuantityByProductAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return (int)await DbSet.Where(p => p.Id == productId).SumAsync(p => p.Quantity, cancellationToken);
    }

    /// <inheritdoc />
    public Task<decimal> GetTotalCostValueAsync(CancellationToken cancellationToken = default)
    {
        return DbSet.SumAsync(p => (p.CostPrice ?? 0) * (decimal)p.Quantity, cancellationToken);
    }

    /// <inheritdoc />
    public Task<decimal> GetTotalSalesValueAsync(CancellationToken cancellationToken = default)
    {
        return DbSet.SumAsync(p => p.SalesPrice * (decimal)p.Quantity, cancellationToken);
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
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

    /// <inheritdoc />
    public Task<List<ProductModel>> GetAllWithSupplierAsync(CancellationToken cancellationToken = default)
    {
        return DbSet
            .Include(p => p.Supplier).ThenInclude(s => s != null ? s.Person : null)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<List<ProductModel>> GetAllForStatsAsync(CancellationToken cancellationToken = default)
    {
        return DbSet
            .Where(p => p.SupplierId.HasValue)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<List<CategoryBreakdownResponse>> GetCategoryBreakdownAsync(CancellationToken cancellationToken = default)
    {
        return DbSet
            .GroupBy(p => new { p.CategoryId, CategoryName = p.Category.Name })
            .Select(g => new CategoryBreakdownResponse(
                g.Key.CategoryId,
                g.Key.CategoryName,
                g.Sum(p => (p.CostPrice ?? 0) * (decimal)p.Quantity),
                g.Sum(p => p.SalesPrice * (decimal)p.Quantity),
                g.Sum(p => p.Quantity)))
            .ToListAsync(cancellationToken);
    }
}