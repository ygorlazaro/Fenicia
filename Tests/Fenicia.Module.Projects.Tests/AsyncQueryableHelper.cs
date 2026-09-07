using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Query;

namespace Fenicia.Module.Projects.Tests;

public static class AsyncQueryableHelper
{
    public static IQueryable<T> AsAsyncQueryable<T>(this IEnumerable<T> source)
    {
        var enumerable = source as List<T> ?? [];
        return new FakeQueryable<T>(enumerable);
    }

    private sealed class FakeQueryable<T> : IOrderedQueryable<T>, IAsyncEnumerable<T>
    {
        internal readonly List<T> Data;

        public FakeQueryable(List<T> data)
        {
            Data = data;
            Expression = Expression.Constant(this);
            Provider = new FakeQueryProvider<T>(data);
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new FakeAsyncEnumerator<T>(Data.GetEnumerator());
        }

        public Expression Expression { get; internal init; }
        public Type ElementType => typeof(T);
        public IQueryProvider Provider { get; }

        public IEnumerator<T> GetEnumerator()
        {
            return Data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    private sealed class FakeQueryProvider<T>(List<T> data) : IAsyncQueryProvider
    {
        public IQueryable CreateQuery(Expression expression)
        {
            return new FakeQueryable<T>(FilterData(expression)) { Expression = expression };
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new FakeQueryable<TElement>(FilterData(expression).Cast<TElement>().ToList())
                { Expression = expression };
        }

        public object? Execute(Expression expression)
        {
            return Execute<object?>(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            var result = ExecuteInternal(expression);

            if (typeof(TResult).IsGenericType && typeof(TResult).GetGenericTypeDefinition() == typeof(Task<>))
            {
                var resultType = typeof(TResult).GetGenericArguments()[0];
                var taskResult = Convert.ChangeType(result, resultType);
                var fromResultMethod = typeof(Task).GetMethod("FromResult", BindingFlags.Public | BindingFlags.Static)!
                    .MakeGenericMethod(resultType);
                return (TResult)fromResultMethod.Invoke(null, [taskResult])!;
            }

            if (result is TResult tr)
            {
                return tr;
            }

            return (TResult)result!;
        }

        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
        {
            return Execute<TResult>(expression);
        }

        private List<T> FilterData(Expression expression)
        {
            if (expression is not MethodCallExpression methodCall)
            {
                return data;
            }

            var method = methodCall.Method;
            var sourceArg = Unwrap(methodCall.Arguments[0]);

            var current = sourceArg is ConstantExpression { Value: FakeQueryable<T> fake }
                ? fake.Data
                : FilterData(sourceArg);

            switch (method.Name)
            {
                case "Include" or "ThenInclude":
                    return current;
                case "Where":
                {
                    var predicate = (LambdaExpression)Unwrap(methodCall.Arguments[1]);
                    return ((IEnumerable<T>)CallLinq(current, "Where", predicate)).ToList();
                }
                case "OrderBy":
                {
                    var keySelector = (LambdaExpression)Unwrap(methodCall.Arguments[1]);
                    return ((IEnumerable<T>)CallLinq(current, "OrderBy", keySelector)).ToList();
                }
                case "OrderByDescending":
                {
                    var keySelector = (LambdaExpression)Unwrap(methodCall.Arguments[1]);
                    return ((IEnumerable<T>)CallLinq(current, "OrderByDescending", keySelector)).ToList();
                }
                case "Skip":
                {
                    var count = (int)Evaluate(Unwrap(methodCall.Arguments[1]))!;
                    return current.Skip(count).ToList();
                }
                case "Take":
                {
                    var count = (int)Evaluate(Unwrap(methodCall.Arguments[1]))!;
                    return current.Take(count).ToList();
                }
            }

            if (method.Name != "Select")
            {
                return current;
            }

            var selector = (LambdaExpression)Unwrap(methodCall.Arguments[1]);
            var selectResult = (IEnumerable<T>)CallLinq(current, "Select", selector);
            return selectResult.ToList();
        }

        private object? ExecuteInternal(Expression expression)
        {
            switch (expression)
            {
                case MethodCallExpression methodCall:
                {
                    var method = methodCall.Method;
                    var sourceArg = Unwrap(methodCall.Arguments[0]);

                    var current = sourceArg is ConstantExpression { Value: FakeQueryable<T> fake }
                        ? fake.Data
                        : ExecuteInternal(sourceArg) as List<T> ?? data;

                    switch (method.Name)
                    {
                        case "Include" or "ThenInclude":
                            return current;
                        case "Count":
                            return current.Count;
                        case "ToList" or "ToListAsync":
                            return current;
                        case "FirstOrDefault" or "FirstOrDefaultAsync":
                            if (methodCall.Arguments.Count == 1)
                            {
                                return current.FirstOrDefault();
                            }
                            var foPredicate = (LambdaExpression)Unwrap(methodCall.Arguments[1]);
                            return CallLinq(current, "FirstOrDefault", foPredicate);
                        case "First" or "FirstAsync":
                            if (methodCall.Arguments.Count == 1)
                            {
                                return current.First();
                            }
                            var firstPredicate = (LambdaExpression)Unwrap(methodCall.Arguments[1]);
                            return CallLinq(current, "First", firstPredicate);
                        case "Any":
                            if (methodCall.Arguments.Count == 1)
                            {
                                return current.Any();
                            }
                            var anyPredicate = (LambdaExpression)Unwrap(methodCall.Arguments[1]);
                            return CallLinq(current, "Any", anyPredicate);
                        case "Where":
                        {
                            var wherePredicate = (LambdaExpression)Unwrap(methodCall.Arguments[1]);
                            return ((IEnumerable<T>)CallLinq(current, "Where", wherePredicate)).ToList();
                        }
                        case "OrderBy":
                        {
                            var keySelector = (LambdaExpression)Unwrap(methodCall.Arguments[1]);
                            return ((IEnumerable<T>)CallLinq(current, "OrderBy", keySelector)).ToList();
                        }
                        case "OrderByDescending":
                        {
                            var keySelector = (LambdaExpression)Unwrap(methodCall.Arguments[1]);
                            return ((IEnumerable<T>)CallLinq(current, "OrderByDescending", keySelector)).ToList();
                        }
                        case "Skip":
                        {
                            var count = (int)Evaluate(Unwrap(methodCall.Arguments[1]))!;
                            return current.Skip(count).ToList();
                        }
                        case "Take":
                        {
                            var count = (int)Evaluate(Unwrap(methodCall.Arguments[1]))!;
                            return current.Take(count).ToList();
                        }
                        case "Select":
                        {
                            var selector = (LambdaExpression)Unwrap(methodCall.Arguments[1]);
                            var selectResult = (IEnumerable<T>)CallLinq(current, "Select", selector);
                            return selectResult.ToList();
                        }
                    }

                    break;
                }
                case LambdaExpression lambda:
                    return lambda.Compile().DynamicInvoke();
                case ConstantExpression constant:
                    return constant.Value;
            }

            throw new NotSupportedException($"Expression not supported: {expression.GetType().Name}");
        }

        private static object CallLinq(IEnumerable<T> source, string methodName, LambdaExpression lambda)
        {
            if (methodName == "FirstOrDefault" || methodName == "First")
            {
                var predicate = (Func<T, bool>)lambda.Compile();
                return methodName == "FirstOrDefault"
                    ? source.FirstOrDefault(predicate)!
                    : source.First(predicate)!;
            }

            if (methodName == "Any")
            {
                var predicate = (Func<T, bool>)lambda.Compile();
                return source.Any(predicate);
            }

            var elementType = typeof(T);
            var queryableMethods = typeof(Queryable).GetMethods(BindingFlags.Public | BindingFlags.Static);
            var method = queryableMethods.First(m => m.Name == methodName && m.GetParameters().Length == 2);

            MethodInfo genericMethod;
            switch (methodName)
            {
                case "Where":
                    genericMethod = method.MakeGenericMethod(elementType);
                    break;
                case "OrderBy" or "OrderByDescending":
                {
                    var keyType = GetLambdaReturnType(lambda);
                    genericMethod = method.MakeGenericMethod(elementType, keyType);
                    break;
                }
                case "Select":
                {
                    var resultType = GetLambdaReturnType(lambda);
                    genericMethod = method.MakeGenericMethod(elementType, resultType);
                    break;
                }
                default:
                    throw new NotSupportedException($"Method not supported: {methodName}");
            }

            var queryableSource = source.AsQueryable();
            return (IEnumerable<T>)genericMethod.Invoke(null, [queryableSource, lambda])!;
        }

        private static Type GetLambdaReturnType(LambdaExpression lambda)
        {
            return lambda.Body.Type;
        }

        private static Expression Unwrap(Expression expression)
        {
            while (expression is UnaryExpression unary)
            {
                expression = unary.Operand;
            }

            return expression;
        }

        private static object? Evaluate(Expression expression)
        {
            return expression switch
            {
                LambdaExpression lambda => lambda.Compile().DynamicInvoke(),
                ConstantExpression constant => constant.Value,
                _ => throw new NotSupportedException($"Cannot evaluate expression: {expression.GetType().Name}")
            };
        }
    }

    private sealed class FakeAsyncEnumerator<T>(IEnumerator<T> enumerator) : IAsyncEnumerator<T>
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
