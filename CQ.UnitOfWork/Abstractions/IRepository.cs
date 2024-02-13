﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CQ.UnitOfWork.Abstractions
{
    public interface IRepository<TEntity> : IFetchRepository<TEntity>
        where TEntity : class
    {
        #region Create entity
        /// <summary>
        /// Saves the entity pass as parameter into the data store with the ORM configured
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
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
        Task<IList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null);

        IList<TEntity> GetAll(Expression<Func<TEntity, bool>>? predicate = null);

        Task<IList<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, bool>>? predicate = null);

        IList<TResult> GetAll<TResult>(Expression<Func<TEntity, bool>>? predicate = null);
        #endregion

        #region Update entity
        Task UpdateByIdAsync(string id, object updates);

        void UpdateById(string id, object updates);

        Task UpdateByPropAsync(string value, object updates, string prop);

        void UpdateByProp(string value, object updates, string prop);
        #endregion

        Task<bool> ExistAsync(Expression<Func<TEntity, bool>> predicate);

        bool Exist(Expression<Func<TEntity, bool>> predicate);
    }
}
