using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Common.Data;

public static class QueryableExtensions
{
    public static IQueryable<T> ApplySort<T>(this IQueryable<T> query, string? sort)
    {
        if (string.IsNullOrWhiteSpace(sort))
        {
            return query;
        }

        var isDescending = sort.StartsWith('-');
        var propertyName = isDescending ? sort[1..] : sort;

        if (string.IsNullOrWhiteSpace(propertyName))
        {
            return query;
        }

        var parameter = Expression.Parameter(typeof(T), "x");
        var property = Expression.PropertyOrField(parameter, propertyName);
        var lambda = Expression.Lambda(property, parameter);

        var methodName = isDescending ? "OrderByDescending" : "OrderBy";
        var result = Expression.Call(
            typeof(Queryable),
            methodName,
            new Type[] { typeof(T), property.Type },
            query.Expression,
            Expression.Quote(lambda));

        return query.Provider.CreateQuery<T>(result);
    }

    public static IQueryable<T> ApplyFilters<T>(this IQueryable<T> query, Dictionary<string, string>? filters)
    {
        if (filters is null || filters.Count == 0)
        {
            return query;
        }

        var parameter = Expression.Parameter(typeof(T), "x");
        Expression? combinedExpression = null;

        foreach (var filter in filters)
        {
            if (string.IsNullOrWhiteSpace(filter.Value))
            {
                continue;
            }

            var property = typeof(T).GetProperty(filter.Key, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase);
            if (property == null || !property.CanRead)
            {
                continue;
            }

            if (property.PropertyType != typeof(string))
            {
                continue;
            }

            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var containsMethod = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) });
            if (containsMethod == null)
            {
                continue;
            }

            var valueConstant = Expression.Constant(filter.Value, typeof(string));
            var containsCall = Expression.Call(propertyAccess, containsMethod, valueConstant);

            combinedExpression = combinedExpression is null ? containsCall : Expression.AndAlso(combinedExpression, containsCall);
        }

        if (combinedExpression is null)
        {
            return query;
        }

        var lambda = Expression.Lambda(combinedExpression, parameter);
        var whereCall = Expression.Call(
            typeof(Queryable),
            nameof(Queryable.Where),
            new Type[] { typeof(T) },
            query.Expression,
            Expression.Quote(lambda));

        return query.Provider.CreateQuery<T>(whereCall);
    }
}
