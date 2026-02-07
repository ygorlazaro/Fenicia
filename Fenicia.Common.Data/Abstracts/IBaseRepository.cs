using System.Linq.Expressions;

namespace Fenicia.Common.Data.Abstracts;

public interface IBaseRepository<T>
    where T : BaseModel
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct);

    Task<List<T>> GetAllAsync(CancellationToken ct, int page = 1, int perPage = 10 );

    void Add(T entity);

    void AddRange(IEnumerable<T> entities);

    void Update(T entity);

    void Delete(T entity);

    void Delete(Guid id);

    Task<int> SaveChangesAsync(CancellationToken ct);

    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken ct);

    Task<int> CountAsync(Expression<Func<T, bool>>? predicate, CancellationToken ct);

    Task<int> CountAsync(CancellationToken ct);
}
