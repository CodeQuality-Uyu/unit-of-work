using MongoDB.Driver;
using System.Linq.Expressions;

namespace CQ.UnitOfWork.MongoDriver.Extensions;
public static class FilterExtension
{
    public static FilterDefinition<TEntity> NullableWhere<TEntity>(this FilterDefinitionBuilder<TEntity> filter, Expression<Func<TEntity, bool>>? predicate)
    {
        return predicate is null ? filter.Where(e => true) : filter.Where(predicate);
    }
}
