using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Common.Data.Repositories;

public abstract class Repository<T>(DbContext context) : IRepository<T>
    where T : BaseModel
{
    protected DbSet<T> DbSet { get; } = context.Set<T>();
    protected DbContext Context => context;

    public virtual async Task<IEnumerable<T>> GetAllAsync(
        int page = 1,
        int perPage = 10,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync(cancellationToken);
    }

    public virtual IQueryable<T> GetAllQuery()
    {
        return DbSet.AsNoTracking().AsQueryable();
    }

    public virtual Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return DbSet.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public virtual async Task<T> InsertAsync(T model, CancellationToken cancellationToken = default)
    {
        model.Created = DateTime.UtcNow;
        await DbSet.AddAsync(model, cancellationToken);
        await SaveChangesAsync(cancellationToken);
        return model;
    }

    public virtual async Task<T?> UpdateAsync(Guid id, T model, CancellationToken cancellationToken = default)
    {
        var existing = await GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return null;
        }

        context.Entry(existing).CurrentValues.SetValues(model);
        existing.Updated = DateTime.UtcNow;
        context.Entry(existing).State = EntityState.Modified;
        await SaveChangesAsync(cancellationToken);

        return existing;
    }

    public virtual async Task<int> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return 0;
        }

        entity.Deleted = DateTime.UtcNow;
        context.Entry(entity).State = EntityState.Modified;
        return await SaveChangesAsync(cancellationToken);
    }

    public virtual async Task<int> DeleteAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        var entities = await DbSet.Where(e => ids.Contains(e.Id)).ToListAsync(cancellationToken);
        if (entities.Count == 0)
        {
            return 0;
        }

        foreach (var entity in entities)
        {
            entity.Deleted = DateTime.UtcNow;
            context.Entry(entity).State = EntityState.Modified;
        }

        return await SaveChangesAsync(cancellationToken);
    }

    public virtual async Task InsertRangeAsync(IEnumerable<T> models, CancellationToken cancellationToken = default)
    {
        var baseModels = models as T[] ?? [];
        foreach (var model in baseModels)
        {
            model.Created = DateTime.UtcNow;
        }

        await DbSet.AddRangeAsync(baseModels, cancellationToken);
        await SaveChangesAsync(cancellationToken);
    }

    public virtual async Task<IEnumerable<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(predicate).ToListAsync(cancellationToken);
    }

    public virtual Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return DbSet.AnyAsync(predicate, cancellationToken);
    }

    public virtual Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return DbSet.CountAsync(cancellationToken);
    }

    public virtual Task<int> CountAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return DbSet.CountAsync(predicate, cancellationToken);
    }

    public virtual Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return context.SaveChangesAsync(cancellationToken);
    }

    public virtual IQueryable<T> Query() => DbSet;
}
