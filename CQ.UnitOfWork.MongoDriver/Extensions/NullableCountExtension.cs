using MongoDB.Driver;
using System.Linq.Expressions;

namespace CQ.UnitOfWork.MongoDriver.Extensions;
public static class NullableCountExtension
{
    public static async Task<long> NullableCountAsync<TEntity>(this IMongoCollection<TEntity> collection, Expression<Func<TEntity, bool>>? predicate)
    {
        return predicate == null
            ? await collection.CountDocumentsAsync(e => true).ConfigureAwait(false)
            : await collection.CountDocumentsAsync(predicate).ConfigureAwait(false);
    }

    public static long NullableCount<TEntity>(this IMongoCollection<TEntity> collection, Expression<Func<TEntity, bool>>? predicate)
    {
        return predicate == null
            ? collection.CountDocuments(e => true)
            : collection.CountDocuments(predicate);
    }
}
