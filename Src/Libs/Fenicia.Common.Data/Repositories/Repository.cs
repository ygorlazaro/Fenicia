using System.Linq.Expressions;
using Fenicia.Common.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Common.Data.Repositories;

public class Repository<T>(DefaultContext context) : IRepository<T>
    where T : BaseModel
{
    protected DbSet<T> DbSet { get; set; } = context.Set<T>();

    public async Task<IEnumerable<T>> GetAllAsync(
        int page = 1,
        int perPage = 10,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(e => e.Deleted == null)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync(cancellationToken);
    }

    public Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return DbSet.FirstOrDefaultAsync(e => e.Id == id && e.Deleted == null, cancellationToken);
    }

    public async Task<T> InsertAsync(T model, CancellationToken cancellationToken = default)
    {
        model.Created = DateTime.UtcNow;
        await DbSet.AddAsync(model, cancellationToken);
        await SaveChangesAsync(cancellationToken);
        return model;
    }

    public async Task<T?> UpdateAsync(Guid id, T model, CancellationToken cancellationToken = default)
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

    public async Task<int> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
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

    public async Task<int> DeleteAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        var entities = await DbSet.Where(e => ids.Contains(e.Id) && e.Deleted == null).ToListAsync(cancellationToken);
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

    public async Task InsertRangeAsync(IEnumerable<T> models, CancellationToken cancellationToken = default)
    {
        var baseModels = models as T[] ?? [];
        foreach (var model in baseModels)
        {
            model.Created = DateTime.UtcNow;
        }

        await DbSet.AddRangeAsync(baseModels, cancellationToken);
        await SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(predicate).ToListAsync(cancellationToken);
    }

    public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return DbSet.AnyAsync(predicate, cancellationToken);
    }

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return DbSet.CountAsync(cancellationToken);
    }

    public Task<int> CountAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return DbSet.CountAsync(predicate, cancellationToken);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return context.SaveChangesAsync(cancellationToken);
    }

    public IQueryable<T> Query()
    {
        // ReSharper disable once UnusedVariable
        var c = context;
        return DbSet;
    }
}
