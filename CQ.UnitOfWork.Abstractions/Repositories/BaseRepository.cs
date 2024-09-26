using CQ.UnitOfWork.Abstractions.Extensions;
using System.Linq.Expressions;

namespace CQ.UnitOfWork.Abstractions.Repositories;
public abstract class BaseRepository<TEntity>
    : IFetchRepository<TEntity>
    where TEntity : class
{
    protected string EntityName => typeof(TEntity).Name;

    #region Abstractions
    public abstract TEntity Get(Expression<Func<TEntity, bool>> predicate);

    public abstract Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate);

    public abstract TEntity GetByProp(string value, string prop);

    public abstract Task<TEntity> GetByPropAsync(string value, string prop);

    public abstract TEntity? GetOrDefault(Expression<Func<TEntity, bool>> predicate);

    public abstract Task<TEntity?> GetOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

    public abstract TEntity? GetOrDefaultByProp(string value, string prop);

    public abstract Task<TEntity?> GetOrDefaultByPropAsync(string value, string prop);

    public abstract Task<TEntity> GetByIdAsync(string id);

    public abstract TEntity GetById(string id);

    public abstract Task<TEntity?> GetOrDefaultByIdAsync(string id);

    public abstract TEntity? GetOrDefaultById(string id);

    public abstract Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null);

    public abstract List<TEntity> GetAll(Expression<Func<TEntity, bool>>? predicate = null);

    public abstract Task<List<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, bool>>? predicate = null)
        where TResult : class;

    public abstract List<TResult> GetAll<TResult>(Expression<Func<TEntity, bool>>? predicate = null)
        where TResult : class;

    public abstract Task<Pagination<TEntity>> GetPagedAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        int page = 1,
        int pageSize = 10);

    public abstract Pagination<TEntity> GetPaged(
        Expression<Func<TEntity, bool>>? predicate = null,
        int page = 1,
        int pageSize = 10);
    #endregion

    public virtual async Task<TEntity> GetAsync<TException>(
        Expression<Func<TEntity, bool>> predicate,
        TException exception)
        where TException : Exception
    {
        return await ExecuteAsync(() => GetAsync(predicate), exception).ConfigureAwait(false);
    }

    private static async Task<TResult> ExecuteAsync<TException, TResult>(
        Func<Task<TResult>> actionAsync,
        TException exception)
        where TResult : class
        where TException : Exception
    {
        try
        {
            return await actionAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            exception.SetInnerException(ex);
            throw exception;
        }
    }

    public virtual TEntity Get<TException>(
        Expression<Func<TEntity, bool>> predicate,
        TException exception)
        where TException : Exception
    {
        return Execute(() => Get(predicate), exception);
    }

    private static TResult Execute<TException, TResult>(
        Func<TResult> action,
        TException exception)
        where TResult : class
        where TException : Exception
    {
        try
        {
            return action();
        }
        catch (Exception ex)
        {
            exception.SetInnerException(ex);
            throw exception;
        }
    }

    public virtual async Task<TEntity> GetByPropAsync<TException>(
        string value,
        string prop,
        TException exception) where TException : Exception
    {
        return await ExecuteAsync(() => GetByPropAsync(value, prop), exception).ConfigureAwait(false);
    }

    public virtual TEntity GetByProp<TException>(
        string value,
        string prop,
        TException exception) where TException : Exception
    {
        return Execute(() => GetByProp(value, prop), exception);
    }

    public virtual async Task<TEntity> GetByIdAsync<TException>(
        string id,
        TException exception) where TException : Exception
    {
        return await ExecuteAsync(() => GetByIdAsync(id), exception).ConfigureAwait(false);
    }

    public TEntity GetById<TException>(
        string id,
        TException exception) where TException : Exception
    {
        return Execute(() => GetById(id), exception);
    }
}
