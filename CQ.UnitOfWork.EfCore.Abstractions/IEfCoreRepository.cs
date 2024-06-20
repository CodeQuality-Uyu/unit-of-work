using CQ.UnitOfWork.Abstractions.Repositories;
using System.Linq.Expressions;

namespace CQ.UnitOfWork.EfCore.Abstractions;
public interface IEfCoreRepository<TEntity> : IRepository<TEntity> where TEntity : class
{
    Task<List<TResult>> GetAllAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null)
    where TResult : class;

    List<TResult> GetAll<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null)
        where TResult : class;
}
