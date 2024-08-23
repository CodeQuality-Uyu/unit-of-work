using System.Linq.Expressions;

namespace CQ.UnitOfWork.Abstractions.Repositories;
public interface IFetchRepository<TEntity>
    where TEntity : class
{
    #region Fetch entities
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

    #region Fetch entity
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

    #region Fetch entity or default
    Task<TEntity?> GetOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

    TEntity? GetOrDefault(Expression<Func<TEntity, bool>> predicate);
    #endregion

    #region Fetch entity by prop
    Task<TEntity> GetByPropAsync(
        string value,
        string prop);

    Task<TEntity> GetByPropAsync<TException>(
        string value,
        string prop,
        TException exception)
        where TException : Exception;

    TEntity GetByProp(
        string value,
        string prop);

    TEntity GetByProp<TException>(
        string value,
        string prop,
        TException exception)
        where TException : Exception;
    #endregion

    #region Fetch entity or default by prop
    Task<TEntity?> GetOrDefaultByPropAsync(
        string value,
        string prop);

    TEntity? GetOrDefaultByProp(
        string value,
        string prop);
    #endregion

    #region Fetch entity by id
    Task<TEntity> GetByIdAsync(string id);

    Task<TEntity> GetByIdAsync<TException>(
        string id,
        TException exception)
        where TException : Exception;

    TEntity GetById(string id);

    TEntity GetById<TException>(
        string id,
        TException exception)
        where TException : Exception;
    #endregion

    #region Fetch entity or default by id
    Task<TEntity?> GetOrDefaultByIdAsync(string id);

    TEntity? GetOrDefaultById(string id);
    #endregion
}
