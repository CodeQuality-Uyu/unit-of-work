using System.Linq.Expressions;

namespace CQ.UnitOfWork.EfCore.Extensions;
public static class NullableWhereExtension
{
    public static IQueryable<T> NullableWhere<T>(
        this IQueryable<T> query,
        Expression<Func<T, bool>>? predicate)
    {
        return predicate == null
            ? query
            : query.Where(predicate);
    }
}
