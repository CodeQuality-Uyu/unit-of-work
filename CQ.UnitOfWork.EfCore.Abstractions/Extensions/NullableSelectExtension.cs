
using System.Linq.Expressions;

namespace CQ.UnitOfWork.EfCore.Abstractions.Extensions;
public static class NullableSelectExtension
{
    public static IQueryable<T> NullableSelect<T>(this IQueryable<T> elements, Expression<Func<T, T>> select)
    {
        return select is null ? elements : elements.Select(select);
    }
}
