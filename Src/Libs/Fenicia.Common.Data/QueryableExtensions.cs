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
        var combinedExpression = (from filter in filters where !string.IsNullOrWhiteSpace(filter.Value) select BuildFilterExpression(parameter, typeof(T), filter.Key, filter.Value)).OfType<Expression>()
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
        Expression? combinedExpression = propertyPaths.Select(propertyPath => BuildFilterExpression(parameter, typeof(T), propertyPath, searchTerm))
            .OfType<Expression>()
            .Aggregate<Expression?, Expression?>(
                null,
                (current, filterExpression) => current is null
                    ? filterExpression
                    : Expression.OrElse(current, filterExpression));

        if (combinedExpression is null)
        {
            return query;
        }

        var lambda = Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);
        return query.Where(lambda);
    }

    private static Expression? BuildFilterExpression(Expression parameter, Type type, string propertyPath, string value)
    {
        var propertyAccess = ResolvePropertyAccess(parameter, type, propertyPath);
        if (propertyAccess is null)
        {
            return null;
        }

        return BuildComparisonExpression(propertyAccess, value);
    }

    private static Expression? ResolvePropertyAccess(Expression parameter, Type type, string propertyPath)
    {
        var parts = propertyPath.Split('.');
        Expression current = parameter;
        Type currentType = type;

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
                var innerExpr = ResolvePropertyAccess(itemParam, elementType, remainingPath);
                if (innerExpr is null)
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

            current = Expression.MakeMemberAccess(current, property);
        }

        return current;
    }

    private static Expression? BuildComparisonExpression(MemberExpression propertyAccess, string value)
    {
        var propertyType = propertyAccess.Type;

        if (propertyType == typeof(string))
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
            var currentLower = Expression.Call(propertyAccess, toLowerMethod);
            return Expression.Call(currentLower, containsMethod, valueConstant);
        }

        if (propertyType == typeof(Guid) && Guid.TryParse(value, out var guidValue))
        {
            var valueConstant = Expression.Constant(guidValue, typeof(Guid));
            return Expression.Equal(propertyAccess, valueConstant);
        }

        if (propertyType == typeof(Guid?) && Guid.TryParse(value, out var nullableGuidValue))
        {
            var valueConstant = Expression.Constant(nullableGuidValue, typeof(Guid));
            return Expression.Equal(propertyAccess, valueConstant);
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
