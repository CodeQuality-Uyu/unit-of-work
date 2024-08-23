namespace CQ.UnitOfWork.EfCore.Extensions;
public static class NullableOrderByExtension
{
    public static IQueryable<T> NullableOrderBy<T>(
        this IQueryable<T> query,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy)
    {
        return orderBy == null
            ? query
            : orderBy(query);
    }
}
