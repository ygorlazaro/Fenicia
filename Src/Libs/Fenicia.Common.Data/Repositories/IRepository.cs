using System.Linq.Expressions;

namespace Fenicia.Common.Data.Repositories;

public interface IRepository<T>
    where T : BaseModel
{
    Task<IEnumerable<T>> GetAllAsync(int page = 1, int perPage = 10, CancellationToken cancellationToken = default);

    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<T> InsertAsync(T model, CancellationToken cancellationToken = default);

    Task<T?> UpdateAsync(Guid id, T model, CancellationToken cancellationToken = default);

    Task<int> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<int> DeleteAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);

    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    Task<int> CountAsync(CancellationToken cancellationToken = default);

    Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task InsertRangeAsync(IEnumerable<T> models, CancellationToken cancellationToken = default);

    IQueryable<T> Query();
}