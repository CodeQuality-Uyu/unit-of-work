using System.Linq.Expressions;

namespace CQ.UnitOfWork.Abstractions.Repositories;
public interface IRepository<TEntity>
    : IFetchRepository<TEntity>
    where TEntity : class
{
    #region Create
    Task<TEntity> CreateAndSaveAsync(TEntity entity);

    TEntity CreateAndSave(TEntity entity);

    Task<List<TEntity>> CreateBulkAndSaveAsync(List<TEntity> entities);

    List<TEntity> CreateBulkAndSave(List<TEntity> entities);

    Task<TEntity> CreateAsync(TEntity entity);

    TEntity Create(TEntity entity);
    #endregion

    #region Delete
    Task DeleteAndSaveAsync(Expression<Func<TEntity, bool>> predicate);

    Task DeleteBulkAndSaveAsync(List<TEntity> entities);

    Task DeleteAndSaveAsync(TEntity entity);

    void DeleteAndSave(Expression<Func<TEntity, bool>> predicate);

    void DeleteAndSave(TEntity entity);
    #endregion

    #region Update
    Task UpdateAndSaveAsync(TEntity entity);

    void UpdateAndSave(TEntity entity);

    Task UpdateAndSaveByIdAsync<TId>(TId id, object updates);

    void UpdateAndSaveById<TId>(TId id, object updates);

    Task UpdateAndSaveByPropAsync<TProp>(TProp value, string prop, object updates);

    void UpdateAndSaveByProp<TProp>(TProp value, string prop, object updates);
    #endregion

    #region Exist

    Task<bool> ExistAsync(Expression<Func<TEntity, bool>> predicate);

    bool Exist(Expression<Func<TEntity, bool>> predicate);
    #endregion
}
