namespace CQ.UnitOfWork.Abstractions.Repositories;
public interface IUnitRepository<TEntity> : IRepository<TEntity>
    where TEntity : class
{
    void SetContext(IDatabaseContext context);

    #region Create entity
    Task CreateWithoutCommitAsync(TEntity entity);

    void CreateWithoutCommit(TEntity entity);
    #endregion
}