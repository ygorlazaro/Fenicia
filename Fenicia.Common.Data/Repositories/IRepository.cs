using System.Linq.Expressions;

namespace Fenicia.Common.Data.Repositories;

public interface IRepository<T> where T : BaseModel
{
    Task<IEnumerable<T>> GetAllAsync(int page = 1, int perPage = 10, CancellationToken ct = default);
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<T> InsertAsync(T model, CancellationToken ct = default);
    Task<T?> UpdateAsync(Guid id, T model, CancellationToken ct = default);
    Task<int> DeleteAsync(Guid id, CancellationToken ct = default);
    Task<int> DeleteAsync(IEnumerable<Guid> ids, CancellationToken ct = default);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
    Task<int> CountAsync(CancellationToken ct = default);
    Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
