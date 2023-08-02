﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using SharpCompress.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CQ.UnitOfWork.Core
{
    public class MongoRepository<TEntity> : IMongoRepository<TEntity> where TEntity : class
    {
        private readonly IMongoCollection<TEntity> _collection;
        private readonly IMongoCollection<BsonDocument> _genericCollection;
        private readonly string CollectionName;

        public MongoRepository(IMongoDatabase mongoDatabase, string? collectionName = null)
        {
            this.CollectionName = string.IsNullOrEmpty(collectionName) ? $"{typeof(TEntity).Name}s" : collectionName;

            this._collection = mongoDatabase.GetCollection<TEntity>(this.CollectionName);
            this._genericCollection = mongoDatabase.GetCollection<BsonDocument>(this.CollectionName);
        }

        public async Task<IList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? expression = null)
        {
            expression ??= e => true;
            var entities = await this._collection.Find(expression).ToListAsync().ConfigureAwait(false);

            return entities;
        }

        public async Task<TEntity?> GetOrDefaultAsync(Expression<Func<TEntity, bool>> expression)
        {
            var entity = await this._collection.Find(expression).FirstOrDefaultAsync().ConfigureAwait(false);

            return entity;
        }

        public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> expression)
        {
            var entity = await this.GetOrDefaultAsync(expression).ConfigureAwait(false);

            if (entity is null)
            {
                throw new Exception($"{this.CollectionName} not found");
            }

            return entity;
        }

        public async Task<TEntity> GetByPropAsync(string value, string? prop = null)
        {
            prop ??= "_id";

            var filter = Builders<BsonDocument>.Filter.Eq(prop, value);

            var entity = await this._genericCollection.Find(filter).FirstOrDefaultAsync().ConfigureAwait(false);

            if (entity is null)
            {
                throw new Exception($"{this.CollectionName} not found");
            }

            return BsonSerializer.Deserialize<TEntity>(entity);
        }

        public async Task<TEntity> CreateAsync(TEntity entity)
        {
            await this._collection.InsertOneAsync(entity).ConfigureAwait(false);

            return entity;
        }

        public async Task DeleteAsync(Expression<Func<TEntity, bool>> expression)
        {
            var deleteResult = await this._collection.DeleteOneAsync(expression).ConfigureAwait(false);
        }
    }
}