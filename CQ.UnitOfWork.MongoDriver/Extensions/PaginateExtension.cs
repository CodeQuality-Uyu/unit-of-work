using MongoDB.Driver;

namespace CQ.UnitOfWork.MongoDriver.Extensions;
public static class PaginateExtension
{
    public static IFindFluent<TEntity, TEntity> Paginate<TEntity>(this IFindFluent<TEntity, TEntity> collection, int page = 1, int pageSize = 10)
    {
        if (page <= 0 || pageSize <= 0)
        {
            return collection;
        }

        var skip = (page - 1) * pageSize;
        var pagedCollection = collection
            .Skip(skip)
            .Limit(pageSize);

        return pagedCollection;
    }
}
