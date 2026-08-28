using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Product;

public class ProductRepository(DefaultContext context) : Repository<ProductModel>(context)
{
    public async Task<IEnumerable<ProductModel>> GetAllWithDetailsAsync(int page = 1, int perPage = 10, CancellationToken ct = default)
    {
        return await DbSet
                .Where(e => e.Deleted == null)
            .Include(p => p.Category)
            .Include(p => p.Supplier).ThenInclude(s => s != null ? s.Person : null)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync(ct);
    }

    public async Task<ProductModel?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default)
    {
        return await DbSet
                .Include(p => p.Category)
            .Include(p => p.Supplier).ThenInclude(s => s != null ? s.Person : null)
            .FirstOrDefaultAsync(e => e.Id == id && e.Deleted == null, ct);
    }

    public async Task<IEnumerable<ProductModel>> GetByCategoryIdAsync(Guid categoryId, int page = 1, int perPage = 10, CancellationToken ct = default)
    {
        return await DbSet
                .Where(p => p.CategoryId == categoryId && p.Deleted == null)
            .Include(p => p.Category)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync(ct);
    }
}
