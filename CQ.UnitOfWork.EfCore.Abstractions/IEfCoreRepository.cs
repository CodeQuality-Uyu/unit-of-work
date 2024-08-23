using CQ.UnitOfWork.Abstractions.Repositories;
using CQ.UnitOfWork.EfCore.Abstractions.Params;
using System.Linq.Expressions;

namespace CQ.UnitOfWork.EfCore.Abstractions;
public interface IEfCoreRepository<TEntity>
    : IRepository<TEntity>
    where TEntity : class
{
    Task<List<TResult>> GetAllSelectAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null)
        where TResult : class;

    List<TResult> GetAllSelect<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null)
        where TResult : class;

    Task<List<TEntity>> GetAllAsync(
        List<IncludeParam<TEntity>> includes,
        Expression<Func<TEntity, bool>>? predicate = null);

    List<TEntity> GetAll(
        List<IncludeParam<TEntity>> includes,
        Expression<Func<TEntity, bool>>? predicate = null);

    Task<Pagination<TResult>> GetPagedAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        int page = 1,
        int pageSize = 10)
        where TResult : class;

    Pagination<TResult> GetPaged<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        int page = 1,
        int pageSize = 10)
        where TResult : class;

    Task<Pagination<TEntity>> GetPagedAsync(
        List<IncludeParam<TEntity>> includes,
        Expression<Func<TEntity, bool>>? predicate = null,
        int page = 1,
        int pageSize = 10);

    Pagination<TEntity> GetPaged(
        List<IncludeParam<TEntity>> includes,
        Expression<Func<TEntity, bool>>? predicate = null,
        int page = 1,
        int pageSize = 10);
}
