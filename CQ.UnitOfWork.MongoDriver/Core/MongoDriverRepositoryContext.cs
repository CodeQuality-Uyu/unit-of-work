namespace CQ.UnitOfWork.MongoDriver.Core;
internal sealed class MongoDriverRepositoryContext<TEntity, TContext>(TContext context) :
    MongoDriverRepository<TEntity>(context)
    where TEntity : class
    where TContext : MongoContext
{
}
