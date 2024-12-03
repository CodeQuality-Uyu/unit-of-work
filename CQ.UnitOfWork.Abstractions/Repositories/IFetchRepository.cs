using System.Linq.Expressions;

namespace CQ.UnitOfWork.Abstractions.Repositories;
public interface IFetchRepository<TEntity>
    where TEntity : class
{
    #region Fetch all
    Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null);

    List<TEntity> GetAll(Expression<Func<TEntity, bool>>? predicate = null);

    Task<List<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, bool>>? predicate = null)
        where TResult : class;

    List<TResult> GetAll<TResult>(Expression<Func<TEntity, bool>>? predicate = null)
        where TResult : class;

    Task<Pagination<TEntity>> GetPagedAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        int page = 1,
        int pageSize = 10);

    Pagination<TEntity> GetPaged(
        Expression<Func<TEntity, bool>>? predicate = null,
        int page = 1,
        int pageSize = 10);
    #endregion

    #region Fetch one
    Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate);

    Task<TEntity> GetAsync<TException>(
        Expression<Func<TEntity, bool>> predicate,
        TException exception)
        where TException : Exception;

    TEntity Get(Expression<Func<TEntity, bool>> predicate);

    TEntity Get<TException>(
        Expression<Func<TEntity, bool>> predicate,
        TException exception)
        where TException : Exception;
    #endregion

    #region Fetch one or default
    Task<TEntity?> GetOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

    TEntity? GetOrDefault(Expression<Func<TEntity, bool>> predicate);
    #endregion

    #region Fetch one by prop
    Task<TEntity> GetByPropAsync<TProp>(
        TProp value,
        string prop);

    Task<TEntity> GetByPropAsync<TException, TProp>(
        TProp value,
        string prop,
        TException exception)
        where TException : Exception;

    TEntity GetByProp<TProp>(
        TProp value,
        string prop);

    TEntity GetByProp<TException, TProp>(
        TProp value,
        string prop,
        TException exception)
        where TException : Exception;
    #endregion

    #region Fetch one or default by prop

    Task<TEntity?> GetOrDefaultByPropAsync<TProp>(
        TProp value,
        string prop);

    TEntity? GetOrDefaultByProp<TProp>(
        TProp value,
        string prop);
    #endregion

    #region Fetch one by id
    Task<TEntity> GetByIdAsync<TId>(TId id);

    Task<TEntity> GetByIdAsync<TException, TId>(
        TId id,
        TException exception)
        where TException : Exception;

    TEntity GetById<TId>(TId id);

    TEntity GetById<TException, TId>(
        TId id,
        TException exception)
        where TException : Exception;
    #endregion

    #region Fetch one or default by id
    Task<TEntity?> GetOrDefaultByIdAsync<TId>(TId id);

    TEntity? GetOrDefaultById<TId>(TId id);
    #endregion
}
