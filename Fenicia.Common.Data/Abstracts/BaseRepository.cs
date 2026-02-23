using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Common.Data.Abstracts;

public class BaseRepository<T>(DbContext context) : IBaseRepository<T>
    where T : BaseModel
{
    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await context.Set<T>().FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public virtual async Task<List<T>> GetAllAsync(CancellationToken ct, int page = 1, int perPage = 10)
    {
        var query = context.Set<T>().Skip((page - 1) * perPage).Take(perPage);

        return await query.ToListAsync(ct);
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

    public virtual async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var model = await GetByIdAsync(id, ct);

        if (model is null) return;

        model.Deleted = DateTime.Now;
        context.Set<T>().Update(model);
    }

    public virtual async Task<int> SaveChangesAsync(CancellationToken ct)
    {
        return await context.SaveChangesAsync(ct);
    }

    public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken ct)
    {
        return await context.Set<T>().AnyAsync(predicate, ct);
    }

    public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate, CancellationToken ct)
    {
        return predicate == null
            ? await context.Set<T>().CountAsync(ct)
            : await context.Set<T>().CountAsync(predicate, ct);
    }

    public async Task<int> CountAsync(CancellationToken ct)
    {
        return await context.Set<T>().CountAsync(ct);
    }
}