using System.Linq.Expressions;

namespace CQ.UnitOfWork.EfCore.Abstractions.Params
{
    public record IncludeParam<TEntity>(Expression<Func<TEntity, object>> Include);
}
