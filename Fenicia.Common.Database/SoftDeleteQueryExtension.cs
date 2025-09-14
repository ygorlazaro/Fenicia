namespace Fenicia.Common.Database;

using System.Linq.Expressions;
using System.Reflection;

using Microsoft.EntityFrameworkCore.Metadata;

public static class SoftDeleteQueryExtension
{
    public static void AddSoftDeleteQueryFilter(this IMutableEntityType entityData)
    {
        var methodToCall = typeof(SoftDeleteQueryExtension).GetMethod(nameof(GetSoftDeleteFilter), BindingFlags.NonPublic | BindingFlags.Static)?.MakeGenericMethod(entityData.ClrType);

        if (methodToCall is null)
        {
            return;
        }

        var filter = methodToCall.Invoke(obj: null, []);

        entityData.SetQueryFilter(filter as LambdaExpression);
    }

    private static LambdaExpression GetSoftDeleteFilter<TEntity>() where TEntity : BaseModel
    {
        return (Expression<Func<TEntity, bool>>)(x => x.Deleted == null);
    }
}
