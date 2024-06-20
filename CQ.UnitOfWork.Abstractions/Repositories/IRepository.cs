using System.Linq.Expressions;

namespace CQ.UnitOfWork.Abstractions.Repositories;
public interface IRepository<TEntity> : IFetchRepository<TEntity>
    where TEntity : class
{
    #region Create entity
    Task<TEntity> CreateAsync(TEntity entity);

    TEntity Create(TEntity entity);

    Task<List<TEntity>> CreateBulkAsync(List<TEntity> entities);

    List<TEntity> CreateBulk(List<TEntity> entities);

    #endregion

    #region Delete entity
    Task DeleteAsync(Expression<Func<TEntity, bool>> predicate);

    void Delete(Expression<Func<TEntity, bool>> predicate);
    #endregion

    #region Fetch entities
    Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null);

    List<TEntity> GetAll(Expression<Func<TEntity, bool>>? predicate = null);

    Task<List<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, bool>>? predicate = null);

    List<TResult> GetAll<TResult>(Expression<Func<TEntity, bool>>? predicate = null);

    Task<Pagination<TEntity>> GetPagedAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        int page = 1,
        int pageSize = 10);

    Pagination<TEntity> GetPaged(
        Expression<Func<TEntity, bool>>? predicate = null,
        int page = 1,
        int pageSize = 10);
    #endregion

    #region Update entity
    Task UpdateAsync(TEntity entity);

    void Update(TEntity entity);

    Task UpdateByIdAsync(string id, object updates);

    void UpdateById(string id, object updates);

    Task UpdateByPropAsync(string value, string prop, object updates);

    void UpdateByProp(string value, string prop, object updates);
    #endregion

    #region Exist entity

    Task<bool> ExistAsync(Expression<Func<TEntity, bool>> predicate);

    bool Exist(Expression<Func<TEntity, bool>> predicate);
    #endregion
}
