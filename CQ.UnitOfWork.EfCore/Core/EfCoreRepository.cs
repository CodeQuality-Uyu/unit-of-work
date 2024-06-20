using CQ.Exceptions;
using CQ.UnitOfWork.Abstractions.Repositories;
using CQ.UnitOfWork.EfCore.Abstractions;
using CQ.UnitOfWork.EfCore.Abstractions.Extensions;
using CQ.UnitOfWork.EfCore.Extensions;
using CQ.Utility;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CQ.UnitOfWork.EfCore.Core;
public class EfCoreRepository<TEntity>(EfCoreContext efCoreContext) :
    BaseRepository<TEntity>,
    IEfCoreRepository<TEntity>
   where TEntity : class
{
    protected DbSet<TEntity> _dbSet = efCoreContext.GetEntitySet<TEntity>();

    protected EfCoreContext _efCoreConnection = efCoreContext;

    #region Create
    public virtual async Task<TEntity> CreateAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity).ConfigureAwait(false);

        await _efCoreConnection.SaveChangesAsync().ConfigureAwait(false);

        return entity;
    }

    public virtual TEntity Create(TEntity entity)
    {
        _dbSet.Add(entity);

        _efCoreConnection.SaveChanges();

        return entity;
    }

    public virtual async Task<List<TEntity>> CreateBulkAsync(List<TEntity> entities)
    {
        await _dbSet.AddRangeAsync(entities).ConfigureAwait(false);

        await _efCoreConnection.SaveChangesAsync().ConfigureAwait(false);

        return entities;
    }

    public virtual List<TEntity> CreateBulk(List<TEntity> entities)
    {
        _dbSet.AddRange(entities);

        _efCoreConnection.SaveChanges();

        return entities;
    }

    #endregion

    #region Delete
    public virtual async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var entitiesToRemove = _dbSet.Where(predicate);

        _dbSet.RemoveRange(entitiesToRemove);

        await _efCoreConnection.SaveChangesAsync().ConfigureAwait(false);
    }

    public virtual void Delete(Expression<Func<TEntity, bool>> predicate)
    {
        var entitiesToRemove = _dbSet.Where(predicate);

        _dbSet.RemoveRange(entitiesToRemove);

        _efCoreConnection.SaveChanges();
    }
    #endregion

    #region GetAll
    public virtual async Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null)
    {
        return await _dbSet
            .NullableWhere(predicate)
            .ToListAsync()
            .ConfigureAwait(false);
    }

    public virtual List<TEntity> GetAll(Expression<Func<TEntity, bool>>? predicate = null)
    {
        return _dbSet.NullableWhere(predicate).ToList();
    }

    public virtual async Task<List<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>>? predicate = null)
        where TResult : class
    {
        return await _dbSet.NullableWhere(predicate).Select(selector).ToListAsync().ConfigureAwait(false);
    }

    public virtual List<TResult> GetAll<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>>? predicate = null)
        where TResult : class
    {
        return _dbSet.NullableWhere(predicate).Select(selector).ToList();
    }

    public virtual async Task<List<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, bool>>? predicate = null)
    {
        return await _dbSet
            .NullableWhere(predicate)
            .SelectTo<TEntity, TResult>()
            .ToListAsync()
            .ConfigureAwait(false);
    }

    public virtual List<TResult> GetAll<TResult>(Expression<Func<TEntity, bool>>? predicate = null)
    {
        return _dbSet.NullableWhere(predicate).SelectTo<TEntity, TResult>().ToList();
    }
    #endregion

    #region GetPaginated
    public virtual async Task<Pagination<TEntity>> GetPagedAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        int page = 1,
        int pageSize = 10)
    {
        return await _dbSet.PaginateAsync(predicate, page, pageSize).ConfigureAwait(false);
    }

    public virtual Pagination<TEntity> GetPaged(
        Expression<Func<TEntity, bool>>? predicate = null,
        int page = 1,
        int pageSize = 10)
    {
        return _dbSet.Paginate(predicate, page, pageSize);
    }
    #endregion

    #region Get
    public override async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var entity = await GetOrDefaultAsync(predicate).ConfigureAwait(false);

        if (Guard.IsNull(entity))
            throw new SpecificResourceNotFoundException<TEntity>("condition", string.Empty);

        return entity;
    }

    public override TEntity Get(Expression<Func<TEntity, bool>> predicate)
    {
        var entity = GetOrDefault(predicate);

        if (Guard.IsNull(entity))
            throw new SpecificResourceNotFoundException<TEntity>("condition", string.Empty);

        return entity;
    }
    #endregion

    #region GetByProp
    public override async Task<TEntity> GetByPropAsync(string value, string prop)
    {
        var entity = await GetOrDefaultByPropAsync(value, prop).ConfigureAwait(false);

        if (Guard.IsNull(entity))
            throw new SpecificResourceNotFoundException<TEntity>(prop, value);

        return entity;
    }

    public override TEntity GetByProp(string value, string prop)
    {
        var entity = GetOrDefaultByProp(value, prop);

        if (Guard.IsNull(entity))
            throw new SpecificResourceNotFoundException<TEntity>(prop, value);

        return entity;
    }
    #endregion

    #region GetOrDefault
    public override async Task<TEntity?> GetOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _dbSet.Where(predicate).FirstOrDefaultAsync().ConfigureAwait(false);
    }

    public override TEntity? GetOrDefault(Expression<Func<TEntity, bool>> predicate)
    {
        return _dbSet.Where(predicate).FirstOrDefault();
    }
    #endregion

    #region GetOrDefaultByProp
    public override async Task<TEntity?> GetOrDefaultByPropAsync(string value, string prop)
    {
        var entity = await GetOrDefaultAsync(e => EF.Property<string>(e, prop) == value).ConfigureAwait(false);

        return entity;
    }

    public override TEntity? GetOrDefaultByProp(string value, string prop)
    {
        var entity = GetOrDefault(e => EF.Property<string>(e, prop) == value);

        return entity;
    }
    #endregion

    #region GetById

    public override async Task<TEntity> GetByIdAsync(string id)
    {
        return await GetByPropAsync(id, "Id").ConfigureAwait(false);
    }

    public override TEntity GetById(string id)
    {
        return GetByProp(id, "Id");
    }

    public override async Task<TEntity?> GetOrDefaultByIdAsync(string id)
    {
        return await GetOrDefaultByPropAsync(id, "Id").ConfigureAwait(false);
    }

    public override TEntity? GetOrDefaultById(string id)
    {
        return GetOrDefaultByProp(id, "Id");
    }
    #endregion

    #region Update
    public virtual async Task UpdateAsync(TEntity updated)
    {
        _dbSet.Update(updated);

        await _efCoreConnection.SaveChangesAsync().ConfigureAwait(false);
    }

    public virtual void Update(TEntity updated)
    {
        _dbSet.Update(updated);

        _efCoreConnection.SaveChanges();
    }

    public virtual async Task UpdateByIdAsync(string id, object updates)
    {
        await UpdateByPropAsync(id, "Id", updates).ConfigureAwait(false);

        await _efCoreConnection.SaveChangesAsync().ConfigureAwait(false);
    }

    public virtual void UpdateById(string id, object updates)
    {
        UpdateByProp(id, "Id", updates);
    }

    public virtual async Task UpdateByPropAsync(string value, string prop, object updates)
    {
        var query = BuildUpdateQuery(updates, prop, value);

        var rawsAffected = 3;// await _efCoreConnection.Database.ExecuteSqlRawAsync(query).ConfigureAwait(false);

        if (rawsAffected != 0)
        {
            var entity = _dbSet.Find(value)!;
            await _efCoreConnection.Entry(entity).ReloadAsync().ConfigureAwait(false);
        }
    }

    private string BuildUpdateQuery(object updates, string id, string idValue)
    {
        var typeofUpdates = updates.GetType();
        var propsOfUpdates = typeofUpdates.GetProperties();
        var namesOfProps = propsOfUpdates.Select(p =>
        {
            var propertyType = p.PropertyType.Name.ToLower();
            var value = p.GetValue(updates);

            if (propertyType == "string")
            {
                return $"{p.Name}='{value}'";
            }

            return $"{p.Name}={value}";
        });
        var updatesSql = string.Join(",", namesOfProps);
        var table = _efCoreConnection.GetTableName<TEntity>();

        var sql = string.Format("UPDATE {0} SET {1} WHERE {2} = '{3}'", table, updatesSql, id, idValue);

        return sql;
    }

    public virtual void UpdateByProp(string value, string prop, object updates)
    {
        var query = BuildUpdateQuery(updates, value, prop);

        var rawsAffected = 3;//_efCoreConnection.Database.ExecuteSqlRaw(query);

        if (rawsAffected != 0)
        {
            var entity = _dbSet.Find(value)!;
            _efCoreConnection.Entry(entity).Reload();
        }

    }
    #endregion

    #region Exist
    public virtual async Task<bool> ExistAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate).ConfigureAwait(false);
    }

    public bool Exist(Expression<Func<TEntity, bool>> predicate)
    {
        return _dbSet.Any(predicate);
    }
    #endregion
}
