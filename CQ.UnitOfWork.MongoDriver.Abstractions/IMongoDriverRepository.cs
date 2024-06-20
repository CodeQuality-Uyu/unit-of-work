using CQ.UnitOfWork.Abstractions.Repositories;

namespace CQ.UnitOfWork.MongoDriver.Abstractions;
public interface IMongoDriverRepository<TEntity> :
    IRepository<TEntity> where TEntity : class
{
}
