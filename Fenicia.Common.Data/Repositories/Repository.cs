using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Common.Data.Repositories;

public abstract class Repository<T>(DbContext context) : IRepository<T>
    where T : BaseModel
{
    protected DbSet<T> DbSet { get; } = context.Set<T>();
    protected DbContext Context => context;

    protected virtual async Task<IEnumerable<T>> GetAllAsync(
        int page = 1,
        int perPage = 10,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync(cancellationToken);
    }

    async Task<IEnumerable<T>> IRepository<T>.GetAllAsync(
        int page,
        int perPage,
        CancellationToken cancellationToken)
    {
        return await GetAllAsync(page, perPage, cancellationToken);
    }

    protected virtual IQueryable<T> GetAllQuery()
    {
        return DbSet.AsNoTracking().AsQueryable();
    }

    IQueryable<T> IRepository<T>.GetAllQuery()
    {
        return GetAllQuery();
    }

    protected virtual Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return DbSet.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    Task<T?> IRepository<T>.GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return GetByIdAsync(id, cancellationToken);
    }

    protected async Task<T> InsertAsync(T model, CancellationToken cancellationToken = default)
    {
        model.Created = DateTime.UtcNow;
        await DbSet.AddAsync(model, cancellationToken);
        await SaveChangesAsync(cancellationToken);
        return model;
    }

    async Task<T> IRepository<T>.InsertAsync(T model, CancellationToken cancellationToken)
    {
        return await InsertAsync(model, cancellationToken);
    }

    protected async Task<T?> UpdateAsync(Guid id, T model, CancellationToken cancellationToken = default)
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

    async Task<T?> IRepository<T>.UpdateAsync(Guid id, T model, CancellationToken cancellationToken)
    {
        return await UpdateAsync(id, model, cancellationToken);
    }

    protected async Task<int> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
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

    async Task<int> IRepository<T>.DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        return await DeleteAsync(id, cancellationToken);
    }

    protected async Task<int> DeleteAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
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

    async Task<int> IRepository<T>.DeleteAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken)
    {
        return await DeleteAsync(ids, cancellationToken);
    }

    protected async Task InsertRangeAsync(IEnumerable<T> models, CancellationToken cancellationToken = default)
    {
        var baseModels = models as T[] ?? [];
        foreach (var model in baseModels)
        {
            model.Created = DateTime.UtcNow;
        }

        await DbSet.AddRangeAsync(baseModels, cancellationToken);
        await SaveChangesAsync(cancellationToken);
    }

    async Task IRepository<T>.InsertRangeAsync(IEnumerable<T> models, CancellationToken cancellationToken)
    {
        await InsertRangeAsync(models, cancellationToken);
    }

    protected async Task<IEnumerable<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(predicate).ToListAsync(cancellationToken);
    }

    async Task<IEnumerable<T>> IRepository<T>.FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken)
    {
        return await FindAsync(predicate, cancellationToken);
    }

    protected Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return DbSet.AnyAsync(predicate, cancellationToken);
    }

    Task<bool> IRepository<T>.AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
    {
        return AnyAsync(predicate, cancellationToken);
    }

    protected Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return DbSet.CountAsync(cancellationToken);
    }

    Task<int> IRepository<T>.CountAsync(CancellationToken cancellationToken)
    {
        return CountAsync(cancellationToken);
    }

    protected Task<int> CountAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return DbSet.CountAsync(predicate, cancellationToken);
    }

    Task<int> IRepository<T>.CountAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken)
    {
        return CountAsync(predicate, cancellationToken);
    }

    protected Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return context.SaveChangesAsync(cancellationToken);
    }

    Task<int> IRepository<T>.SaveChangesAsync(CancellationToken cancellationToken)
    {
        return SaveChangesAsync(cancellationToken);
    }

    protected IQueryable<T> Query() => DbSet;

    IQueryable<T> IRepository<T>.Query()
    {
        return Query();
    }
}
