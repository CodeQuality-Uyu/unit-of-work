using Microsoft.EntityFrameworkCore.Query;

namespace CQ.UnitOfWork.EfCore.Extensions;

public static class NullableIncludeExtension
{
    public static IQueryable<T> NullableInclude<T>(
        this IQueryable<T> query,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include)
    {
        return include == null
            ? query
            : include(query);
    }
}
