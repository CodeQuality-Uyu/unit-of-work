namespace CQ.UnitOfWork.EfCore.Core;
public class EfCoreRepositoryContext<TEntity, TContext>(TContext context) 
    : EfCoreRepository<TEntity>(context)
    where TEntity : class
    where TContext : EfCoreContext
{
    protected TContext ConcreteContext { get; } = context;
}
