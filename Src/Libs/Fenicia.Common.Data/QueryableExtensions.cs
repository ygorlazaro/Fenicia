using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

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
        var property = ResolvePropertyPath(parameter, propertyName);
        if (property is null)
        {
            return query;
        }

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

            var filterExpression = BuildFilterExpression(parameter, typeof(T), filter.Key, filter.Value);
            if (filterExpression != null)
            {
                combinedExpression = combinedExpression is null
                    ? filterExpression
                    : Expression.AndAlso(combinedExpression, filterExpression);
            }
        }

        if (combinedExpression is null)
        {
            return query;
        }

        var lambda = Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);
        return query.Where(lambda);
    }

    public static IQueryable<T> ApplySearch<T>(this IQueryable<T> query, string? searchTerm, params string[] propertyPaths)
    {
        if (string.IsNullOrWhiteSpace(searchTerm) || propertyPaths.Length == 0)
        {
            return query;
        }

        var parameter = Expression.Parameter(typeof(T), "x");
        Expression? combinedExpression = null;

        foreach (var propertyPath in propertyPaths)
        {
            var filterExpression = BuildFilterExpression(parameter, typeof(T), propertyPath, searchTerm);
            if (filterExpression != null)
            {
                combinedExpression = combinedExpression is null
                    ? filterExpression
                    : Expression.OrElse(combinedExpression, filterExpression);
            }
        }

        if (combinedExpression is null)
        {
            return query;
        }

        var lambda = Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);
        return query.Where(lambda);
    }

    private static Expression? BuildFilterExpression(Expression parameter, Type type, string propertyPath, string value)
    {
        var parts = propertyPath.Split('.');
        var current = parameter;
        var currentType = type;

        for (var i = 0; i < parts.Length; i++)
        {
            var property = currentType.GetProperty(
                             parts[i],
                             BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (property == null || !property.CanRead)
            {
                return null;
            }

            currentType = property.PropertyType;

            if (IsCollectionType(currentType, out var elementType))
            {
                if (i == parts.Length - 1)
                {
                    return null;
                }

                var remainingPath = string.Join(".", parts.Skip(i + 1));
                var itemParam = Expression.Parameter(elementType, "item");
                var innerExpr = BuildFilterExpression(itemParam, elementType, remainingPath, value);
                if (innerExpr == null)
                {
                    return null;
                }

                var propertyAccess = Expression.MakeMemberAccess(current, property);
                var anyMethod = typeof(Enumerable)
                    .GetMethods(BindingFlags.Static | BindingFlags.Public)
                    .First(m => m.Name == nameof(Enumerable.Any) && m.GetParameters().Length == 2)
                    .MakeGenericMethod(elementType);

                var lambda = Expression.Lambda(innerExpr, itemParam);
                return Expression.Call(anyMethod, new Expression[] { propertyAccess, lambda });
            }

            current = Expression.MakeMemberAccess(current, property);
        }

        if (currentType == typeof(string))
        {
            var toLowerMethod = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes);
            if (toLowerMethod == null)
            {
                return null;
            }

            var containsMethod = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) });
            if (containsMethod == null)
            {
                return null;
            }

            var valueConstant = Expression.Constant(value.ToLowerInvariant(), typeof(string));
            var currentLower = Expression.Call(current, toLowerMethod);
            return Expression.Call(currentLower, containsMethod, valueConstant);
        }

        if (currentType == typeof(Guid))
        {
            if (!Guid.TryParse(value, out var guidValue))
            {
                return null;
            }

            var valueConstant = Expression.Constant(guidValue, typeof(Guid));
            return Expression.Equal(current, valueConstant);
        }

        if (currentType == typeof(Guid?))
        {
            if (!Guid.TryParse(value, out var guidValue))
            {
                return null;
            }

            var valueConstant = Expression.Constant(guidValue, typeof(Guid));
            return Expression.Equal(current, valueConstant);
        }

        return null;
    }

    private static bool IsCollectionType(Type type, out Type elementType)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ICollection<>))
        {
            elementType = type.GetGenericArguments()[0];
            return true;
        }

        if (type.IsGenericType && type.GetGenericArguments().Length == 1
            && typeof(IEnumerable).IsAssignableFrom(type) && !type.IsGenericTypeDefinition)
        {
            elementType = type.GetGenericArguments()[0];
            return true;
        }

        elementType = null!;
        return false;
    }

    private static MemberExpression? ResolvePropertyPath(ParameterExpression parameter, string propertyName)
    {
        var parts = propertyName.Split('.');
        Expression current = parameter;
        MemberExpression? property = null;

        foreach (var part in parts)
        {
            property = Expression.PropertyOrField(current, part);
            current = property;
        }

        return property;
    }
}
