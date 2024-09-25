using CQ.UnitOfWork.Abstractions.Repositories;

namespace CQ.UnitOfWork.Abstractions;

public interface IUnitOfWork
{
    IRepository<TEntity> GetEntityRepository<TEntity>() where TEntity : class;

    TRepository GetRepository<TRepository, TEntity>()
        where TRepository : class
        where TEntity : class;

    Task CommitChangesAsync();
}