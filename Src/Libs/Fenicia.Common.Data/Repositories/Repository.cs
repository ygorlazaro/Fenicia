using System.Linq.Expressions;
using Fenicia.Common.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Common.Data.Repositories;

public class Repository<T> : IRepository<T>
    where T : BaseModel
{
    public Repository(DefaultContext context)
    {
        DbSet = context.Set<T>();
        Context = context;
    }

    public Repository()
    {
    }

    public DefaultContext Context { get; set; } = null!;

    protected DbSet<T> DbSet { get; set; } = null!;

    public async Task<IEnumerable<T>> GetAllAsync(int page = 1, int perPage = 10, CancellationToken ct = default)
    {
        return await DbSet
                .Where(e => e.Deleted == null)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync(ct);
    }

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await DbSet.FirstOrDefaultAsync(e => e.Id == id && e.Deleted == null, ct);
    }

    public async Task<T> InsertAsync(T model, CancellationToken ct = default)
    {
        model.Created = DateTime.UtcNow;
        await DbSet.AddAsync(model, ct);
        await SaveChangesAsync(ct);
        return model;
    }

    public async Task<T?> UpdateAsync(Guid id, T model, CancellationToken ct = default)
    {
        var existing = await GetByIdAsync(id, ct);
        if (existing is null)
        {
            return null;
        }

        Context.Entry(existing).CurrentValues.SetValues(model);
        existing.Updated = DateTime.UtcNow;
        await SaveChangesAsync(ct);
        return existing;
    }

    public async Task<int> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await GetByIdAsync(id, ct);
        if (entity is null)
        {
            return 0;
        }

        entity.Deleted = DateTime.UtcNow;
        return await SaveChangesAsync(ct);
    }

    public async Task<int> DeleteAsync(IEnumerable<Guid> ids, CancellationToken ct = default)
    {
        var entities = await DbSet.Where(e => ids.Contains(e.Id) && e.Deleted == null).ToListAsync(ct);
        if (entities.Count == 0)
        {
            return 0;
        }

        foreach (var entity in entities)
        {
            entity.Deleted = DateTime.UtcNow;
        }

        return await SaveChangesAsync(ct);
    }

    public async Task InsertRangeAsync(IEnumerable<T> models, CancellationToken ct = default)
    {
        foreach (var model in models)
        {
            model.Created = DateTime.UtcNow;
        }

        await DbSet.AddRangeAsync(models, ct);
        await SaveChangesAsync(ct);
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
    {
        return await DbSet.Where(predicate).ToListAsync(ct);
    }

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
    {
        return await DbSet.AnyAsync(predicate, ct);
    }

    public async Task<int> CountAsync(CancellationToken ct = default)
    {
        return await DbSet.CountAsync(ct);
    }

    public async Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
    {
        return await DbSet.CountAsync(predicate, ct);
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return await Context.SaveChangesAsync(ct);
    }

    public IQueryable<T> Query()
    {
        return DbSet;
    }
}
