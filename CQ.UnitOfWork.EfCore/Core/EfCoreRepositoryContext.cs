namespace CQ.UnitOfWork.EfCore.Core;
internal sealed class EfCoreRepositoryContext<TEntity, TContext>(TContext context) :
    EfCoreRepository<TEntity>(context)
    where TEntity : class
    where TContext : EfCoreContext
{
}
