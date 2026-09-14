using System.Linq.Expressions;

namespace Fenicia.Module.Projects.Tests;

internal sealed class TestAsyncEnumerable<T>(Expression expression)
    : EnumerableQuery<T>(expression), IAsyncEnumerable<T>, IQueryable<T>
{
    IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        using var enumerator = this.AsEnumerable().GetEnumerator();
        return new TestAsyncEnumerator<T>(enumerator);
    }
}