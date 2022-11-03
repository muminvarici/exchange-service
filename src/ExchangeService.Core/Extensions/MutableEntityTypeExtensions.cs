using System.Linq.Expressions;
using System.Reflection;
using ExchangeService.Core.Entities;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ExchangeService.Core.Extensions;

public static class MutableEntityTypeExtensions
{
    public static void AddSoftDeleteQueryFilter(
        this IMutableEntityType entityData)
    {
        var methodToCall = typeof(MutableEntityTypeExtensions)
            .GetMethod(nameof(GetSoftDeleteFilter),
                BindingFlags.NonPublic | BindingFlags.Static)
            .MakeGenericMethod(entityData.ClrType);
        var filter = methodToCall.Invoke(null, new object[] { });
        entityData.SetQueryFilter((LambdaExpression)filter);
        //entityData.AddIndex(entityData.FindProperty(nameof(EntityBase.RecordStatus)));
    }

    private static LambdaExpression GetSoftDeleteFilter<TEntity>()
        where TEntity : EntityBase
    {
        Expression<Func<TEntity, bool>> filter = x => x.RecordStatus == 'A';
        return filter;
    }
}