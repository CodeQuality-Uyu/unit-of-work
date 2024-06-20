using MongoDB.Driver;
using System.Linq.Expressions;

namespace CQ.UnitOfWork.MongoDriver.Extensions;

public static class FindExtension
{
    public static IFindFluent<TEntity, TEntity> NullableFind<TEntity>(this IMongoCollection<TEntity> collection, Expression<Func<TEntity, bool>>? predicate)
    {
        return predicate == null ? collection.Find(e => true) : collection.Find(predicate);
    }
}
