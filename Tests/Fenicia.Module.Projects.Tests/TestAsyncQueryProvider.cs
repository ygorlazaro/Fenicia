using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace Fenicia.Module.Projects.Tests;

public sealed class TestAsyncQueryProvider<T>(IQueryProvider inner) : IAsyncQueryProvider
{
    public IQueryable CreateQuery(Expression expression)
    {
        return new TestAsyncEnumerable<T>(expression);
    }

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
    {
        return new TestAsyncEnumerable<TElement>(expression);
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
        return inner.Execute<TResult>(expression);
    }
}