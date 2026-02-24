using System.Linq.Expressions;
using System.Reflection;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Fenicia.Common.Data;

public static class SoftDeleteQueryExtension
{
    public static void AddSoftDeleteSupport(ModelBuilder modelBuilder)
    {
        var mutableEntityTypes = modelBuilder.Model.GetEntityTypes()
            .Where(entityType => typeof(BaseModel).IsAssignableFrom(entityType.ClrType));

        foreach (var entityType in mutableEntityTypes)
        {
            entityType.AddSoftDeleteQueryFilter();
        }
    }

    private static void AddSoftDeleteQueryFilter(this IMutableEntityType entityData)
    {
        var methodToCall = typeof(SoftDeleteQueryExtension)
            .GetMethod(nameof(GetSoftDeleteFilter), BindingFlags.NonPublic | BindingFlags.Static)
            ?.MakeGenericMethod(entityData.ClrType);

        if (methodToCall is null)
        {
            return;
        }

        var filter = methodToCall.Invoke(null, []);

        entityData.SetQueryFilter(filter as LambdaExpression);
    }

    private static LambdaExpression GetSoftDeleteFilter<TEntity>()
        where TEntity : BaseModel
    {
        return (Expression<Func<TEntity, bool>>)(x => x.Deleted == null);
    }
}