using System.Linq.Expressions;
using System.Reflection;

namespace Fenicia.Common;

public static class AdvancedQueryExtensions
{
    public static IQueryable<T> ApplyAdvancedQuery<T>(this IQueryable<T> query, List<QueryFilter> filters, string? sort = null)
    {
        if (filters is { Count: > 0 })
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            Expression? combined = null;

            foreach (var filter in filters)
            {
                var property = typeof(T).GetProperty(filter.Property, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (property is null)
                {
                    continue;
                }

                var member = Expression.Property(parameter, property);
                var constant = Expression.Constant(Convert.ChangeType(filter.Value, property.PropertyType));
                var body = (Expression)(filter.Operator switch
                {
                    QueryOperator.Equals => Expression.Equal(member, constant),
                    QueryOperator.NotEquals => Expression.NotEqual(member, constant),
                    QueryOperator.Contains => Expression.Call(member, property.PropertyType.GetMethod("Contains", [property.PropertyType])!, constant),
                    QueryOperator.GreaterThan => Expression.GreaterThan(member, constant),
                    QueryOperator.LessThan => Expression.LessThan(member, constant),
                    QueryOperator.GreaterThanOrEqual => Expression.GreaterThanOrEqual(member, constant),
                    QueryOperator.LessThanOrEqual => Expression.LessThanOrEqual(member, constant),
                    _ => Expression.Equal(member, constant)
                });

                combined = combined is null ? body : Expression.AndAlso(combined, body);
            }

            if (combined is not null)
            {
                var lambda = Expression.Lambda<Func<T, bool>>(combined, parameter);
                query = query.Where(lambda);
            }
        }

        if (string.IsNullOrWhiteSpace(sort))
        {
            return query;
        }

        {
            var sortParts = sort.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var first = true;

            foreach (var sortPart in sortParts)
            {
                var isDescending = sortPart.StartsWith('-');
                var propertyName = isDescending ? sortPart[1..] : sortPart;
                var property = typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (property is null)
                {
                    continue;
                }

                var parameter = Expression.Parameter(typeof(T), "x");
                var member = Expression.Property(parameter, property);
                var lambda = Expression.Lambda(member, parameter);

                var methodName = first ? (isDescending ? nameof(Queryable.OrderByDescending) : nameof(Queryable.OrderBy)) : (isDescending ? nameof(Queryable.ThenByDescending) : nameof(Queryable.ThenBy));
                var method = typeof(Queryable).GetMethods()
                    .First(m => m.Name == methodName && m.GetParameters().Length == 2)
                    .MakeGenericMethod(typeof(T), property.PropertyType);

                query = (IQueryable<T>)method.Invoke(null, [query, lambda])!;
                first = false;
            }
        }

        return query;
    }
}
