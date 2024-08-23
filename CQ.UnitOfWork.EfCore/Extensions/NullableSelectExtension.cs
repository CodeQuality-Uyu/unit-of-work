using System.Linq.Expressions;

namespace CQ.UnitOfWork.EfCore.Extensions;
public static class NullableSelectExtension
{
    public static IQueryable<T> NullableSelect<T>(
        this IQueryable<T> query,
        Expression<Func<T, T>>? select)
    {
        return select == null
            ? query
            : query.Select(select);
    }
}
