namespace CQ.UnitOfWork.EfCore.Core;
public class EfCoreRepositoryContext<TEntity, TContext>(TContext _context) 
    : EfCoreRepository<TEntity>(_context)
    where TEntity : class
    where TContext : EfCoreContext
{
    protected TContext ConcreteContext { get; } = _context;
}
