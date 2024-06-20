

namespace CQ.UnitOfWork.EfCore.Abstractions.Extensions;
public static class NullableOrderByExtension
{
    public static IQueryable<T> NullableOrderBy<T>(this IQueryable<T> elements, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy)
    {
        return orderBy is null ? elements : orderBy(elements);
    }
}
