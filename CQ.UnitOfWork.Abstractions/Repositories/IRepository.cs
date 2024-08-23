using System.Linq.Expressions;

namespace CQ.UnitOfWork.Abstractions.Repositories;
public interface IRepository<TEntity>
    : IFetchRepository<TEntity>
    where TEntity : class
{
    #region Create entity
    Task<TEntity> CreateAsync(TEntity entity);

    TEntity Create(TEntity entity);

    Task<List<TEntity>> CreateBulkAsync(List<TEntity> entities);

    List<TEntity> CreateBulk(List<TEntity> entities);

    #endregion

    #region Delete entity
    Task DeleteAndSaveAsync(Expression<Func<TEntity, bool>> predicate);

    Task DeleteBulkAndSaveAsync(List<TEntity> entities);

    Task DeleteAndSaveAsync(TEntity entity);

    void DeleteAndSave(Expression<Func<TEntity, bool>> predicate);

    void DeleteAndSave(TEntity entity);
    #endregion

    #region Update entity
    Task UpdateAndSaveAsync(TEntity entity);

    void UpdateAndSave(TEntity entity);

    Task UpdateAndSaveByIdAsync(string id, object updates);

    void UpdateAndSaveById(string id, object updates);

    Task UpdateAndSaveByPropAsync(string value, string prop, object updates);

    void UpdateAndSaveByProp(string value, string prop, object updates);
    #endregion

    #region Exist entity

    Task<bool> ExistAsync(Expression<Func<TEntity, bool>> predicate);

    bool Exist(Expression<Func<TEntity, bool>> predicate);
    #endregion
}
