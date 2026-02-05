using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Common.Database.Abstracts;

public class BaseRepository<T>(DbContext context) : IBaseRepository<T>
    where T : BaseModel, new()
{
    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.Set<T>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public virtual async Task<List<T>> GetAllAsync(CancellationToken cancellationToken, int page = 1, int perPage = 10)
    {
        return await context.Set<T>().Skip((page - 1) * perPage).Take(perPage).ToListAsync(cancellationToken);
    }

    public virtual void Add(T entity)
    {
        context.Set<T>().Add(entity);
    }

    public void AddRange(IEnumerable<T> entities)
    {
        context.Set<T>().AddRange(entities);
    }

    public virtual void Update(T entity)
    {
        context.Entry(entity).State = EntityState.Modified;
    }

    public virtual void Delete(T entity)
    {
        context.Set<T>().Remove(entity);
    }

    public virtual void Delete(Guid id)
    {
        var entity = new T { Id = id };
        context.Set<T>().Attach(entity);
        context.Set<T>().Remove(entity);
    }

    public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }

    public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
    {
        return await context.Set<T>().AnyAsync(predicate, cancellationToken);
    }

    public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate, CancellationToken cancellationToken)
    {
        if (predicate == null)
        {
            return await context.Set<T>().CountAsync(cancellationToken);
        }

        return await context.Set<T>().CountAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken)
    {
        return await context.Set<T>().CountAsync(cancellationToken);
    }
}
