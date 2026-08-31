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

    public async Task<IEnumerable<T>> GetAllAsync(int page = 1, int perPage = 10, CancellationToken cancellationToken = default)
    {
        return await DbSet
                .Where(e => true)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync(cancellationToken);
    }

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
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

        Context.Entry(existing).CurrentValues.SetValues(model);
        existing.Updated = DateTime.UtcNow;
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

        Context.Entry(entity).State = EntityState.Deleted;
        return await SaveChangesAsync(cancellationToken);
    }

    public async Task<int> DeleteAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        var entities = await DbSet.Where(e => ids.Contains(e.Id)).ToListAsync(cancellationToken);
        if (entities.Count == 0)
        {
            return 0;
        }

        foreach (var entity in entities)
        {
            Context.Entry(entity).State = EntityState.Deleted;
        }

        return await SaveChangesAsync(cancellationToken);
    }

    public async Task InsertRangeAsync(IEnumerable<T> models, CancellationToken cancellationToken = default)
    {
        foreach (var model in models)
        {
            model.Created = DateTime.UtcNow;
        }

        await DbSet.AddRangeAsync(models, cancellationToken);
        await SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.CountAsync(cancellationToken);
    }

    public async Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await DbSet.CountAsync(predicate, cancellationToken);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await Context.SaveChangesAsync(cancellationToken);
    }

    public IQueryable<T> Query()
    {
        return DbSet;
    }
}
