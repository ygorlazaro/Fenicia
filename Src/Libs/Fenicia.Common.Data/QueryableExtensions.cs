using System.Collections;
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
            [typeof(T), property.Type],
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
        var combinedExpression =
            (from filter in filters
                where !string.IsNullOrWhiteSpace(filter.Value)
                select BuildFilterExpression(parameter, typeof(T), filter.Key, filter.Value)).OfType<Expression>()
            .Aggregate<Expression?, Expression?>(
                null,
                (current, filterExpression) => current is null
                    ? filterExpression
                    : Expression.AndAlso(current, filterExpression ?? throw new ArgumentNullException(nameof(filterExpression))));

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
        var combinedExpression = propertyPaths.Select(propertyPath => BuildFilterExpression(parameter, typeof(T), propertyPath, searchTerm))
            .OfType<Expression>()
            .Aggregate<Expression?, Expression?>(
                null,
                (current, filterExpression) =>
                {
                    if (filterExpression != null)
                    {
                        return current is null
                            ? filterExpression
                            : Expression.OrElse(current, filterExpression);
                    }

                    return null;
                });

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
                return BuildCollectionFilter(current, property, elementType, parts, i + 1, value);
            }

            current = Expression.MakeMemberAccess(current, property);
        }

        return BuildLeafExpression(current, currentType, value);
    }

    private static MethodCallExpression? BuildCollectionFilter(
        Expression current,
        PropertyInfo property,
        Type elementType,
        string[] parts,
        int nextIndex,
        string value)
    {
        if (nextIndex >= parts.Length)
        {
            return null;
        }

        var remainingPath = string.Join(".", parts.Skip(nextIndex));
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
        return Expression.Call(anyMethod, [propertyAccess, lambda]);
    }

    private static Expression? BuildLeafExpression(Expression current, Type currentType, string value)
    {
        if (currentType == typeof(string))
        {
            return BuildStringContainsExpression(current, value);
        }

        if (currentType == typeof(Guid) || currentType == typeof(Guid?))
        {
            return BuildGuidEqualsExpression(current, value);
        }

        return null;
    }

    private static MethodCallExpression? BuildStringContainsExpression(Expression current, string value)
    {
        var toLowerMethod = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes);
        if (toLowerMethod == null)
        {
            return null;
        }

        var containsMethod = typeof(string).GetMethod(nameof(string.Contains), [typeof(string)]);
        if (containsMethod == null)
        {
            return null;
        }

        var valueConstant = Expression.Constant(value.ToLowerInvariant(), typeof(string));
        var currentLower = Expression.Call(current, toLowerMethod);
        return Expression.Call(currentLower, containsMethod, valueConstant);
    }

    private static BinaryExpression? BuildGuidEqualsExpression(Expression current, string value)
    {
        if (!Guid.TryParse(value, out var guidValue))
        {
            return null;
        }

        var valueConstant = Expression.Constant(guidValue, typeof(Guid));
        return Expression.Equal(current, valueConstant);
    }

    private static bool IsCollectionType(Type type, out Type elementType)
    {
        if ((type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ICollection<>)) || (type.IsGenericType && type.GetGenericArguments().Length == 1
                && typeof(IEnumerable).IsAssignableFrom(type) && !type.IsGenericTypeDefinition))
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
