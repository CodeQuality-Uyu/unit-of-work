using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CQ.UnitOfWork.EfCore.Extensions;

public static class NullableCountExtension
{
    public static async Task<int> NullableCountAsync<T>(
        this IQueryable<T> query,
        Expression<Func<T, bool>>? predicate)
    {
        return predicate == null
            ? await query.CountAsync().ConfigureAwait(false)
            : await query.CountAsync(predicate).ConfigureAwait(false);
    }

    public static int NullableCount<T>(
        this IQueryable<T> query,
        Expression<Func<T, bool>>? predicate)
    {
        return predicate == null
            ? query.Count()
            : query.Count(predicate);
    }
}
