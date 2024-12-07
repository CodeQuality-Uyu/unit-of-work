using CQ.Exceptions;
using CQ.UnitOfWork.Abstractions.Repositories;
using CQ.UnitOfWork.EfCore.Abstractions;
using CQ.UnitOfWork.EfCore.Abstractions.Params;
using CQ.UnitOfWork.EfCore.Extensions;
using CQ.Utility;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CQ.UnitOfWork.EfCore.Core;
public class EfCoreRepository<TEntity>(EfCoreContext _baseContext) :
    BaseRepository<TEntity>,
    IEfCoreRepository<TEntity>
   where TEntity : class
{
    protected DbSet<TEntity> Entities { get; } = _baseContext.GetEntitySet<TEntity>();

    protected EfCoreContext BaseContext { get; } = _baseContext;

    #region Create
    public virtual async Task<TEntity> CreateAndSaveAsync(TEntity entity)
    {
        await CreateAsync(entity).ConfigureAwait(false);

        await BaseContext.SaveChangesAsync().ConfigureAwait(false);

        return entity;
    }

    public virtual async Task<TEntity> CreateAsync(TEntity entity)
    {
        await Entities
            .AddAsync(entity)
            .ConfigureAwait(false);

        return entity;
    }

    public virtual TEntity CreateAndSave(TEntity entity)
    {
        Create(entity);

        BaseContext.SaveChanges();

        return entity;
    }

    public virtual TEntity Create(TEntity entity)
    {
        Entities.Add(entity);

        return entity;
    }

    public virtual async Task<List<TEntity>> CreateBulkAndSaveAsync(List<TEntity> entities)
    {
        await Entities.AddRangeAsync(entities).ConfigureAwait(false);

        await BaseContext.SaveChangesAsync().ConfigureAwait(false);

        return entities;
    }

    public virtual List<TEntity> CreateBulkAndSave(List<TEntity> entities)
    {
        Entities.AddRange(entities);

        BaseContext.SaveChanges();

        return entities;
    }

    #endregion

    #region Delete
    public virtual async Task DeleteAndSaveAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var entitiesToRemove = Entities.Where(predicate);

        Entities.RemoveRange(entitiesToRemove);

        await BaseContext
            .SaveChangesAsync()
            .ConfigureAwait(false);
    }

    public virtual async Task DeleteBulkAndSaveAsync(List<TEntity> entities)
    {
        Entities.RemoveRange(entities);

        await BaseContext
            .SaveChangesAsync()
            .ConfigureAwait(false);
    }

    public virtual async Task DeleteAndSaveAsync(TEntity entity)
    {
        Entities.Remove(entity);

        await BaseContext
            .SaveChangesAsync()
            .ConfigureAwait(false);
    }

    public virtual void DeleteAndSave(Expression<Func<TEntity, bool>> predicate)
    {
        var entitiesToRemove = Entities.Where(predicate);

        Entities.RemoveRange(entitiesToRemove);

        BaseContext.SaveChanges();
    }

    public virtual void DeleteBulkAndSave(List<TEntity> entities)
    {
        Entities.RemoveRange();

        BaseContext.SaveChanges();
    }

    public virtual void DeleteAndSave(TEntity entity)
    {
        Entities.Remove(entity);

        BaseContext.SaveChanges();
    }
    #endregion

    #region GetAll
    public override async Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null)
    {
        return await Entities
            .NullableWhere(predicate)
            .ToListAsync()
            .ConfigureAwait(false);
    }

    public override List<TEntity> GetAll(Expression<Func<TEntity, bool>>? predicate = null)
    {
        return Entities
            .NullableWhere(predicate)
            .ToList();
    }

    public virtual async Task<List<TResult>> GetAllSelectAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null)
        where TResult : class
    {
        return await Entities
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
        return Entities
            .NullableWhere(predicate)
            .Select(selector)
            .ToList();
    }

    public override async Task<List<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, bool>>? predicate = null)
        where TResult : class
    {
        return await Entities
            .NullableWhere(predicate)
            .SelectTo<TEntity, TResult>()
            .ToListAsync()
            .ConfigureAwait(false);
    }

    public override List<TResult> GetAll<TResult>(Expression<Func<TEntity, bool>>? predicate = null)
        where TResult : class
    {
        return Entities
            .NullableWhere(predicate)
            .SelectTo<TEntity, TResult>()
            .ToList();
    }

    public async Task<List<TEntity>> GetAllAsync(
        List<IncludeParam<TEntity>> includes,
        Expression<Func<TEntity, bool>>? predicate = null)
    {
        var query = Entities.NullableWhere(predicate);

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
        return await Entities
            .ToNullablePaginateAsync(
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
        var task = GetPagedAsync(predicate, page, pageSize);

        var elements = task.Result;

        return elements;
    }

    public async Task<Pagination<TResult>> GetPagedAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        int page = 1,
        int pageSize = 10) where TResult : class
    {
        var elements = await Entities
            .NullableWhere(predicate)
            .Select(selector)
            .ToPaginateAsync(
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
        var query = Entities.NullableWhere(predicate);

        includes.ForEach(include =>
        {
            query = query.Include(include.Include);
        });

        var elements = await query
            .ToPaginateAsync(
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
    public override async Task<TEntity> GetByPropAsync<TProp>(TProp value, string prop)
    {
        var entity = await GetOrDefaultByPropAsync(value, prop).ConfigureAwait(false);

        if (Guard.IsNull(entity))
            throw new SpecificResourceNotFoundException<TEntity>(prop, value.ToString());

        return entity;
    }

    public override TEntity GetByProp<TProp>(TProp value, string prop)
    {
        var entity = GetOrDefaultByProp(value, prop);

        if (Guard.IsNull(entity))
            throw new SpecificResourceNotFoundException<TEntity>(prop, value.ToString());

        return entity;
    }
    #endregion

    #region GetOrDefault
    public override async Task<TEntity?> GetOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await Entities.Where(predicate).FirstOrDefaultAsync().ConfigureAwait(false);
    }

    public override TEntity? GetOrDefault(Expression<Func<TEntity, bool>> predicate)
    {
        return Entities.Where(predicate).FirstOrDefault();
    }
    #endregion

    #region GetOrDefaultByProp
    public override async Task<TEntity?> GetOrDefaultByPropAsync<TProp>(TProp value, string prop)
    {
        var entity = await GetOrDefaultAsync(e => EF.Property<TProp>(e, prop).Equals(value)).ConfigureAwait(false);

        return entity;
    }

    public override TEntity? GetOrDefaultByProp<TProp>(TProp value, string prop)
    {
        var entity = GetOrDefault(e => EF.Property<TProp>(e, prop).Equals(value));

        return entity;
    }
    #endregion

    #region GetById

    public override async Task<TEntity> GetByIdAsync<TId>(TId id)
    {
        return await GetByPropAsync(id, "Id").ConfigureAwait(false);
    }

    public override TEntity GetById<TId>(TId id)
    {
        return GetByProp(id, "Id");
    }

    public override async Task<TEntity?> GetOrDefaultByIdAsync<TId>(TId id)
    {
        return await GetOrDefaultByPropAsync(id, "Id").ConfigureAwait(false);
    }

    public override TEntity? GetOrDefaultById<TId>(TId id)
    {
        return GetOrDefaultByProp(id, "Id");
    }
    #endregion

    #region Update
    public virtual async Task UpdateAndSaveAsync(TEntity updated)
    {
        Entities.Update(updated);

        await BaseContext
            .SaveChangesAsync()
            .ConfigureAwait(false);
    }

    public virtual void UpdateAndSave(TEntity updated)
    {
        Entities.Update(updated);

        BaseContext.SaveChanges();
    }

    public virtual async Task UpdateAndSaveByIdAsync<TId>(
        TId id,
        object updates)
    {
        await UpdateAndSaveByPropAsync(
            id,
            "Id",
            updates)
            .ConfigureAwait(false);

        await BaseContext
            .SaveChangesAsync()
            .ConfigureAwait(false);
    }

    public virtual void UpdateAndSaveById<TId>(
        TId id,
        object updates)
    {
        UpdateAndSaveByProp(
            id,
            "Id",
            updates);
    }

    public virtual async Task UpdateAndSaveByPropAsync<TProp>(
        TProp value,
        string prop,
        object updates)
    {
        var query = BuildUpdateQuery(
            updates,
            prop,
            value);

        var rawsAffected = await BaseContext.Database.ExecuteSqlRawAsync(query).ConfigureAwait(false);

        if (rawsAffected != 0)
        {
            var entity = Entities.Find(value)!;
            await BaseContext
                .Entry(entity)
                .ReloadAsync()
                .ConfigureAwait(false);
        }
    }

    private string BuildUpdateQuery<TId>(
        object updates,
        string id,
        TId idValue)
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
        var table = BaseContext.GetTableName<TEntity>();

        var sql = string.Format("UPDATE {0} SET {1} WHERE {2} = '{3}'", table, updatesSql, id, idValue);

        return sql;
    }

    public virtual void UpdateAndSaveByProp<TProp>(TProp value, string prop, object updates)
    {
        var query = BuildUpdateQuery(updates, prop, value);

        var rawsAffected = 3;//_efCoreConnection.Database.ExecuteSqlRaw(query);

        if (rawsAffected != 0)
        {
            var entity = Entities.Find(value)!;
            BaseContext.Entry(entity).Reload();
        }

    }
    #endregion

    #region Exist
    public virtual async Task<bool> ExistAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await Entities
            .AnyAsync(predicate)
            .ConfigureAwait(false);
    }

    public bool Exist(Expression<Func<TEntity, bool>> predicate)
    {
        return Entities.Any(predicate);
    }
    #endregion

    public static void AssertNullEntity<TProp>(
       TEntity? entity,
       TProp propertyValue,
       string propertyName)
    {
        if (Guard.IsNotNull(entity))
        {
            return;
        }

        throw new SpecificResourceNotFoundException<TEntity>(propertyName, propertyValue.ToString());
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
}
