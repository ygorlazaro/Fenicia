using System.Collections;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace Fenicia.Auth.Tests;

public static class AsyncQueryableHelper
{
    public static IQueryable<T> AsAsyncQueryable<T>(this IEnumerable<T> source)
    {
        var enumerable = source as List<T> ?? [];
        var queryable = enumerable.AsQueryable();
        var provider = new TestAsyncQueryProvider(queryable.Provider);
        return new TestAsyncEnumerable<T>(enumerable, provider);
    }

    private sealed class TestAsyncQueryProvider(IQueryProvider inner) : IAsyncQueryProvider
    {
        public IQueryable CreateQuery(Expression expression)
        {
            return new TestAsyncEnumerable<object>(expression, this);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new TestAsyncEnumerable<TElement>(expression, this);
        }

        public object? Execute(Expression expression)
        {
            return inner.Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return inner.Execute<TResult>(expression);
        }

        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
        {
            return Execute<TResult>(expression);
        }
    }

    private sealed class TestAsyncEnumerable<T> : IQueryable<T>, IAsyncEnumerable<T>
    {
        private readonly IEnumerable<T>? _source;
        public Expression Expression { get; }
        public Type ElementType => typeof(T);

        public TestAsyncEnumerable(IEnumerable<T> source, IAsyncQueryProvider provider)
        {
            _source = source;
            Provider = provider;
            Expression = Expression.Constant(this);
        }

        public TestAsyncEnumerable(Expression expression, IAsyncQueryProvider provider)
        {
            Expression = expression;
            Provider = provider;
        }

        public IQueryProvider Provider { get; }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new TestAsyncEnumerator<T>(_source!.GetEnumerator());
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _source!.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    private sealed class TestAsyncEnumerator<T>(IEnumerator<T> enumerator) : IAsyncEnumerator<T>
    {
        public ValueTask DisposeAsync()
        {
            enumerator.Dispose();
            return default;
        }

        public ValueTask<bool> MoveNextAsync()
        {
            return new ValueTask<bool>(enumerator.MoveNext());
        }

        public T Current => enumerator.Current;
    }
}
