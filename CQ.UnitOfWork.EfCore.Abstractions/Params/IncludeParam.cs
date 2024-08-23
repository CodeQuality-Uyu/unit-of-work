using System.Linq.Expressions;

namespace CQ.UnitOfWork.EfCore.Abstractions.Params
{
    public record IncludeParam<TEntity>(
        Expression<Func<TEntity, object>> Include,
        Expression<Func<TEntity, object>>? ThenInclude = null);
}
