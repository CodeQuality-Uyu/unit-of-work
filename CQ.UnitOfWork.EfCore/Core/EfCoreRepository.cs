using CQ.Exceptions;
using CQ.UnitOfWork.Abstractions;
using CQ.UnitOfWork.Abstractions.Repositories;
using CQ.UnitOfWork.EfCore.Abstractions;
using CQ.UnitOfWork.EfCore.Abstractions.Params;
using CQ.UnitOfWork.EfCore.Extensions;
using CQ.Utility;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CQ.UnitOfWork.EfCore.Core;
public class EfCoreRepository<TEntity>(EfCoreContext baseContext) :
    BaseRepository<TEntity>,
    IEfCoreRepository<TEntity>,
    IUnitRepository<TEntity>
   where TEntity : class
{
    protected DbSet<TEntity> _entities = baseContext.GetEntitySet<TEntity>();

    protected EfCoreContext _baseContext = baseContext;

    #region Create
    public virtual async Task<TEntity> CreateAndSaveAsync(TEntity entity)
    {
        await CreateAsync(entity).ConfigureAwait(false);

        await baseContext.SaveChangesAsync().ConfigureAwait(false);

        return entity;
    }

    public virtual async Task<TEntity> CreateAsync(TEntity entity)
    {
        await _entities
            .AddAsync(entity)
            .ConfigureAwait(false);

        return entity;
    }

    public virtual TEntity CreateAndSave(TEntity entity)
    {
        Create(entity);

        baseContext.SaveChanges();

        return entity;
    }

    public virtual TEntity Create(TEntity entity)
    {
        _entities.Add(entity);

        return entity;
    }

    public virtual async Task<List<TEntity>> CreateBulkAndSaveAsync(List<TEntity> entities)
    {
        await _entities.AddRangeAsync(entities).ConfigureAwait(false);

        await baseContext.SaveChangesAsync().ConfigureAwait(false);

        return entities;
    }

    public virtual List<TEntity> CreateBulkAndSave(List<TEntity> entities)
    {
        _entities.AddRange(entities);

        baseContext.SaveChanges();

        return entities;
    }

    #endregion

    #region Delete
    public virtual async Task DeleteAndSaveAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var entitiesToRemove = _entities.Where(predicate);

        _entities.RemoveRange(entitiesToRemove);

        await baseContext
            .SaveChangesAsync()
            .ConfigureAwait(false);
    }

    public virtual async Task DeleteBulkAndSaveAsync(List<TEntity> entities)
    {
        _entities.RemoveRange(entities);

        await baseContext
            .SaveChangesAsync()
            .ConfigureAwait(false);
    }

    public virtual async Task DeleteAndSaveAsync(TEntity entity)
    {
        _entities.Remove(entity);

        await baseContext
            .SaveChangesAsync()
            .ConfigureAwait(false);
    }

    public virtual void DeleteAndSave(Expression<Func<TEntity, bool>> predicate)
    {
        var entitiesToRemove = _entities.Where(predicate);

        _entities.RemoveRange(entitiesToRemove);

        baseContext.SaveChanges();
    }

    public virtual void DeleteBulkAndSave(List<TEntity> entities)
    {
        _entities.RemoveRange();

        baseContext.SaveChanges();
    }

    public virtual void DeleteAndSave(TEntity entity)
    {
        _entities.Remove(entity);

        baseContext.SaveChanges();
    }
    #endregion

    #region GetAll
    public override async Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null)
    {
        return await _entities
            .NullableWhere(predicate)
            .ToListAsync()
            .ConfigureAwait(false);
    }

    public override List<TEntity> GetAll(Expression<Func<TEntity, bool>>? predicate = null)
    {
        return _entities
            .NullableWhere(predicate)
            .ToList();
    }

    public virtual async Task<List<TResult>> GetAllSelectAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null)
        where TResult : class
    {
        return await _entities
            .NullableWhere(predicate)
            .Select(selector)
            .ToListAsync()
            .ConfigureAwait(false);
    }

    public virtual List<TResult> GetAllSelect<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null)
        where TResult : class
    {
        return _entities
            .NullableWhere(predicate)
            .Select(selector)
            .ToList();
    }

    public override async Task<List<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, bool>>? predicate = null)
        where TResult : class
    {
        return await _entities
            .NullableWhere(predicate)
            .SelectTo<TEntity, TResult>()
            .ToListAsync()
            .ConfigureAwait(false);
    }

    public override List<TResult> GetAll<TResult>(Expression<Func<TEntity, bool>>? predicate = null)
        where TResult : class
    {
        return _entities
            .NullableWhere(predicate)
            .SelectTo<TEntity, TResult>()
            .ToList();
    }

    public async Task<List<TEntity>> GetAllAsync(
        List<IncludeParam<TEntity>> includes,
        Expression<Func<TEntity, bool>>? predicate = null)
    {
        var query = _entities.NullableWhere(predicate);

        includes.ForEach(include =>
        {
            query = query.Include(include.Include);
        });

        var elements = await query
            .ToListAsync()
            .ConfigureAwait(false);

        return elements;
    }

    public List<TEntity> GetAll(
        List<IncludeParam<TEntity>> includes,
        Expression<Func<TEntity, bool>>? predicate = null)
    {
        var task = GetAllAsync(
            includes,
            predicate);

        var elements = task.Result;

        return elements;
    }
    #endregion

    #region GetPaginated
    public override async Task<Pagination<TEntity>> GetPagedAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        int page = 1,
        int pageSize = 10)
    {
        return await _entities
            .PaginateAsync(
            predicate,
            page,
            pageSize)
            .ConfigureAwait(false);
    }

    public override Pagination<TEntity> GetPaged(
        Expression<Func<TEntity, bool>>? predicate = null,
        int page = 1,
        int pageSize = 10)
    {
        return _entities.Paginate(
            predicate,
            page,
            pageSize);
    }

    public async Task<Pagination<TResult>> GetPagedAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        int page = 1,
        int pageSize = 10) where TResult : class
    {
        var elements = await _entities
            .NullableWhere(predicate)
            .Select(selector)
            .PaginateAsync(
            null,
            page,
            pageSize)
            .ConfigureAwait(false);

        return elements;
    }

    public Pagination<TResult> GetPaged<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        int page = 1,
        int pageSize = 10) where TResult : class
    {
        var task = GetPagedAsync(
            selector,
            predicate,
            page,
            pageSize);

        var elements = task.Result;

        return elements;
    }

    public async Task<Pagination<TEntity>> GetPagedAsync(
        List<IncludeParam<TEntity>> includes,
        Expression<Func<TEntity, bool>>? predicate = null,
        int page = 1,
        int pageSize = 10)
    {
        var query = _entities.NullableWhere(predicate);

        includes.ForEach(include =>
        {
            query = query.Include(include.Include);
        });

        var elements = await query
            .PaginateAsync(
            null,
            page,
            pageSize)
            .ConfigureAwait(false);

        return elements;
    }

    public Pagination<TEntity> GetPaged(
        List<IncludeParam<TEntity>> includes,
        Expression<Func<TEntity, bool>>? predicate = null,
        int page = 1,
        int pageSize = 10)
    {
        var task = GetPagedAsync(
            includes,
            predicate,
            page,
            pageSize);

        var elements = task.Result;

        return elements;
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
        return await _entities.Where(predicate).FirstOrDefaultAsync().ConfigureAwait(false);
    }

    public override TEntity? GetOrDefault(Expression<Func<TEntity, bool>> predicate)
    {
        return _entities.Where(predicate).FirstOrDefault();
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
    public virtual async Task UpdateAndSaveAsync(TEntity updated)
    {
        _entities.Update(updated);

        await baseContext
            .SaveChangesAsync()
            .ConfigureAwait(false);
    }

    public virtual void UpdateAndSave(TEntity updated)
    {
        _entities.Update(updated);

        baseContext.SaveChanges();
    }

    public virtual async Task UpdateAndSaveByIdAsync(
        string id,
        object updates)
    {
        await UpdateAndSaveByPropAsync(
            id,
            "Id",
            updates)
            .ConfigureAwait(false);

        await baseContext
            .SaveChangesAsync()
            .ConfigureAwait(false);
    }

    public virtual void UpdateAndSaveById(
        string id,
        object updates)
    {
        UpdateAndSaveByProp(
            id,
            "Id",
            updates);
    }

    public virtual async Task UpdateAndSaveByPropAsync(
        string value,
        string prop,
        object updates)
    {
        var query = BuildUpdateQuery(
            updates,
            prop,
            value);

        var rawsAffected = 3;// await _efCoreConnection.Database.ExecuteSqlRawAsync(query).ConfigureAwait(false);

        if (rawsAffected != 0)
        {
            var entity = _entities.Find(value)!;
            await baseContext
                .Entry(entity)
                .ReloadAsync()
                .ConfigureAwait(false);
        }
    }

    private string BuildUpdateQuery(
        object updates,
        string id,
        string idValue)
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
        var table = baseContext.GetTableName<TEntity>();

        var sql = string.Format("UPDATE {0} SET {1} WHERE {2} = '{3}'", table, updatesSql, id, idValue);

        return sql;
    }

    public virtual void UpdateAndSaveByProp(string value, string prop, object updates)
    {
        var query = BuildUpdateQuery(updates, value, prop);

        var rawsAffected = 3;//_efCoreConnection.Database.ExecuteSqlRaw(query);

        if (rawsAffected != 0)
        {
            var entity = _entities.Find(value)!;
            baseContext.Entry(entity).Reload();
        }

    }
    #endregion

    #region Exist
    public virtual async Task<bool> ExistAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _entities
            .AnyAsync(predicate)
            .ConfigureAwait(false);
    }

    public bool Exist(Expression<Func<TEntity, bool>> predicate)
    {
        return _entities.Any(predicate);
    }
    #endregion

    public static void AssertNullEntity(
       TEntity? entity,
       string propertyValue,
       string propertyName)
    {
        if (Guard.IsNotNull(entity))
        {
            return;
        }

        throw new SpecificResourceNotFoundException<TEntity>(propertyName, propertyValue);
    }

    public static void AssertNullEntity(
        TEntity? entity,
        Exception exception)
    {
        if (Guard.IsNotNull(entity))
        {
            return;
        }

        throw exception;
    }

    public void SetContext(IDatabaseContext context)
    {
        _baseContext = (EfCoreContext)context;
        _entities = _baseContext.GetEntitySet<TEntity>();
    }
}
