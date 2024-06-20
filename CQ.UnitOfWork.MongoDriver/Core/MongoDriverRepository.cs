using CQ.Exceptions;
using CQ.UnitOfWork.Abstractions.Repositories;
using CQ.UnitOfWork.MongoDriver.Abstractions;
using CQ.UnitOfWork.MongoDriver.Extensions;
using CQ.Utility;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System.Linq.Expressions;
using System.Text.Json;

namespace CQ.UnitOfWork.MongoDriver.Core;
public class MongoDriverRepository<TEntity>(MongoContext _mongoContext) :
    BaseRepository<TEntity>,
    IMongoDriverRepository<TEntity>
    where TEntity : class
{
    protected IMongoCollection<TEntity> _collection = _mongoContext.GetEntityCollection<TEntity>();
    protected IMongoCollection<BsonDocument> _genericCollection = _mongoContext.GetGenericCollection<TEntity>();

    #region Create
    public virtual async Task<TEntity> CreateAsync(TEntity entity)
    {
        await _collection.InsertOneAsync(entity).ConfigureAwait(false);

        return entity;
    }

    public virtual TEntity Create(TEntity entity)
    {
        _collection.InsertOne(entity);

        return entity;
    }

    public virtual async Task<List<TEntity>> CreateBulkAsync(List<TEntity> entities)
    {
        await _collection.InsertManyAsync(entities).ConfigureAwait(false);

        return entities;
    }

    public virtual List<TEntity> CreateBulk(List<TEntity> entities)
    {
        _collection.InsertMany(entities);

        return entities;
    }

    #endregion

    #region Delete
    public virtual async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var deleteResult = await _collection.DeleteOneAsync(predicate).ConfigureAwait(false);
    }

    public virtual void Delete(Expression<Func<TEntity, bool>> predicate)
    {
        _collection.DeleteOne(predicate);
    }
    #endregion

    #region Fetch all
    public virtual async Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null)
    {
        return await _collection.NullableFind(predicate).ToListAsync().ConfigureAwait(false);
    }

    public virtual List<TEntity> GetAll(Expression<Func<TEntity, bool>>? predicate)
    {
        return _collection.NullableFind(predicate).ToList();
    }

    public virtual async Task<List<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, bool>>? predicate = null)
    {
        var filter = Builders<TEntity>.Filter.NullableWhere(predicate);

        var projection = Builders<TEntity>.Projection.ProjectTo<TEntity, TResult>();

        var cursor = await _collection
            .Find(filter)
            .Project(projection)
            .ToListAsync()
            .ConfigureAwait(false);

        return cursor;
    }

    public virtual List<TResult> GetAll<TResult>(Expression<Func<TEntity, bool>>? predicate = null)
    {
        var filter = Builders<TEntity>.Filter.NullableWhere(predicate);

        var projection = Builders<TEntity>.Projection.ProjectTo<TEntity, TResult>();

        var cursor = _collection
            .Find(filter)
            .Project(projection)
            .ToList();

        return cursor;
    }
    #endregion

    #region Fetch paginate
    public virtual async Task<Pagination<TEntity>> GetPagedAsync(
        Expression<Func<TEntity, bool>>? predicate,
        int page = 1,
        int pageSize = 10)
    {
        var itemsPaged = await _collection
            .NullableFind(predicate)
            .Paginate(page, pageSize)
            .ToListAsync()
            .ConfigureAwait(false);

        var totalItems = await _collection
            .NullableCountAsync(predicate)
            .ConfigureAwait(false);

        double itemsPerPage = pageSize == 0 ? totalItems : pageSize;
        var totalPages = Convert.ToInt64(Math.Ceiling(totalItems / itemsPerPage));

        return new Pagination<TEntity>(
            itemsPaged,
            totalItems,
            totalPages);
    }

    public virtual Pagination<TEntity> GetPaged(
        Expression<Func<TEntity, bool>>? predicate,
        int page = 1,
        int pageSize = 10)
    {
        var itemsPaged = _collection
            .NullableFind(predicate)
            .Paginate(page, pageSize)
            .ToList();

        var totalItems = _collection
            .NullableCount(predicate);

        double itemsPerPage = pageSize == 0 ? totalItems : pageSize;
        var totalPages = Convert.ToInt64(Math.Ceiling(totalItems / itemsPerPage));

        return new Pagination<TEntity>(
            itemsPaged,
            totalItems,
            totalPages);
    }
    #endregion

    #region Update
    public virtual async Task UpdateAsync(TEntity entity)
    {
        var idValue = GetIdValue(entity);

        var filter = Builders<TEntity>.Filter.Eq("_id", idValue);
        var options = new ReplaceOptions { IsUpsert = true };

        await _collection.ReplaceOneAsync(filter, entity, options).ConfigureAwait(false);
    }

    private static string GetIdValue(TEntity entity)
    {
        var typeEntity = entity.GetType();
        var properties = typeEntity.GetProperties();
        var idProperty = properties.First(p => p.Name == "Id");
        var idValue = idProperty.GetValue(entity)!;

        return idValue.ToString()!;
    }

    public virtual void Update(TEntity entity)
    {
        var idValue = GetIdValue(entity);

        var filter = Builders<TEntity>.Filter.Eq("_id", idValue);
        var options = new ReplaceOptions { IsUpsert = true };

        _collection.ReplaceOne(filter, entity, options);
    }

    public virtual async Task UpdateByIdAsync(string id, object updates)
    {
        await UpdateByPropAsync(id, "_id", updates).ConfigureAwait(false);
    }

    public virtual async Task UpdateByPropAsync(string value, string prop, object updates)
    {
        var filter = BuildFilterByProp(value, prop);
        var updateDefinition = BuildUpdateDefinition(updates);

        var updateResult = await _genericCollection.UpdateOneAsync(filter, updateDefinition).ConfigureAwait(false);
    }

    private static FilterDefinition<BsonDocument> BuildFilterByProp(string value, string prop)
    {
        var filter = Builders<BsonDocument>.Filter.Eq(prop, value);

        return filter;
    }

    private static UpdateDefinition<BsonDocument> BuildUpdateDefinition(object updates)
    {
        var updatesJson = JsonSerializer.Serialize(updates);
        var updatesDocument = BsonDocument.Parse(updatesJson);


        UpdateDefinition<BsonDocument>? update = null;
        foreach (var change in updatesDocument)
        {
            if (update == null)
            {
                var builder = Builders<BsonDocument>.Update;
                update = builder.Set(change.Name, change.Value);
            }
            else
            {
                update = update.Set(change.Name, change.Value);
            }
        }
        return update;
    }

    public virtual void UpdateById(string id, object updates)
    {
        UpdateByProp(id, "_id", updates);
    }

    public virtual void UpdateByProp(string value, string prop, object updates)
    {
        var filter = BuildFilterByProp(value, prop);
        var updateDefinition = BuildUpdateDefinition(updates);

        var updateResult = _genericCollection.UpdateOne(filter, updateDefinition);
    }
    #endregion

    #region Exist
    public async Task<bool> ExistAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var amount = await _collection.CountAsync(predicate).ConfigureAwait(false);

        return amount > 0;
    }

    public bool Exist(Expression<Func<TEntity, bool>> predicate)
    {
        var amount = _collection.Count(predicate);

        return amount > 0;
    }
    #endregion

    #region Unity
    public virtual async Task CreateWithoutCommitAsync(TEntity entity)
    {
        await Task.Run(() => _mongoContext.AddActionAsync(async () => await _collection.InsertOneAsync(entity).ConfigureAwait(false))).ConfigureAwait(false);
    }

    public virtual void CreateWithoutCommit(TEntity entity)
    {
        _mongoContext.AddAction(() => _collection.InsertOne(entity));
    }
    #endregion

    #region Fetch single
    public override async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var entity = await GetOrDefaultAsync(predicate).ConfigureAwait(false);

        if (Guard.IsNull(entity))
        {
            throw new SpecificResourceNotFoundException<TEntity>("condition", string.Empty);
        }

        return entity;
    }

    public override TEntity Get(Expression<Func<TEntity, bool>> predicate)
    {
        var entity = GetOrDefault(predicate);

        if (Guard.IsNull(entity))
        {
            throw new SpecificResourceNotFoundException<TEntity>("condition", string.Empty);
        }

        return entity;
    }

    public override async Task<TEntity> GetByPropAsync(string value, string prop)
    {
        var entity = await GetOrDefaultByPropAsync(value, prop).ConfigureAwait(false);

        if (Guard.IsNull(entity))
        {
            throw new SpecificResourceNotFoundException<TEntity>(prop, value);
        }

        return entity;
    }

    public override TEntity GetByProp(string value, string prop)
    {
        var entity = GetOrDefaultByProp(value, prop);

        if (Guard.IsNull(entity))
        {
            throw new SpecificResourceNotFoundException<TEntity>(prop, value);
        }

        return entity;
    }

    public override async Task<TEntity?> GetOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var entity = await _collection.Find(predicate).FirstOrDefaultAsync().ConfigureAwait(false);

        return entity;
    }

    public override TEntity? GetOrDefault(Expression<Func<TEntity, bool>> predicate)
    {
        return _collection.Find(predicate).FirstOrDefault();
    }

    public override async Task<TEntity?> GetOrDefaultByPropAsync(string value, string prop)
    {
        var filter = Builders<BsonDocument>.Filter.Eq(prop, value);

        var entity = await _genericCollection.Find(filter).FirstOrDefaultAsync().ConfigureAwait(false);

        if (entity == null)
        {
            return null;
        }

        return BsonSerializer.Deserialize<TEntity>(entity);
    }

    public override TEntity? GetOrDefaultByProp(string value, string prop)
    {
        var filter = Builders<BsonDocument>.Filter.Eq(prop, value);

        var entity = _genericCollection.Find(filter).FirstOrDefault();

        if (entity == null)
        {
            return null;
        }

        return BsonSerializer.Deserialize<TEntity>(entity);
    }

    public override async Task<TEntity> GetByIdAsync(string id)
    {
        return await GetByPropAsync(id, "_id").ConfigureAwait(false);
    }

    public override TEntity GetById(string id)
    {
        return GetByProp(id, "_id");
    }

    public override async Task<TEntity?> GetOrDefaultByIdAsync(string id)
    {
        return await GetOrDefaultByPropAsync(id, "_id").ConfigureAwait(false);
    }

    public override TEntity? GetOrDefaultById(string id)
    {
        return GetOrDefaultByProp(id, "_id");
    }

    #endregion
}
