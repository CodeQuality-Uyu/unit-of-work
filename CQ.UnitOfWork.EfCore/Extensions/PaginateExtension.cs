using CQ.UnitOfWork.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;

namespace CQ.UnitOfWork.EfCore.Extensions
{
    public static class PaginateExtension
    {
        public static IQueryable<T> Paginate<T>(
            this IQueryable<T> query,
            int page,
            int pageSize)
        {
            if (page <= 0 || pageSize <= 0)
            {
                return query;
            }

            var skipTo = (page - 1) * pageSize;

            return query
                .Skip(skipTo)
                .Take(pageSize);
        }

        #region Paginate

        public static Pagination<T> ToNullablePaginate<T>(
            this IQueryable<T> query,
            Expression<Func<T, bool>>? predicate,
            int page,
            int pageSize)
        {
            return predicate != null
                ? ToPaginate(query, predicate, page, pageSize)
                : ToPaginate(query, page, pageSize);
        }

        public static Pagination<T> ToPaginate<T>(
            this IQueryable<T> query,
            Expression<Func<T, bool>> predicate,
            int page,
            int pageSize)
        {
            var itemsPaged = query
                .Where(predicate)
                .Paginate(page, pageSize)
                .ToList();

            return BuildPagination(query, predicate, page, pageSize, itemsPaged);
        }

        private static Pagination<T> BuildPagination<T>(IQueryable<T> query, Expression<Func<T, bool>>? predicate, int page, int pageSize, List<T> itemsPaged)
        {
            var totalItems = query
                            .NullableCount(predicate);

            double itemsPerPage = pageSize <= 0
                ? totalItems
                : pageSize;

            var totalPages = Convert.ToInt64(Math.Ceiling(totalItems / itemsPerPage));

            return new Pagination<T>(
                itemsPaged,
                totalItems,
                totalPages,
                page,
                pageSize);
        }

        public static Pagination<T> ToPaginate<T>(
            this IQueryable<T> query,
            int page,
            int pageSize)
        {
            var itemsPaged = query
                .Paginate(page, pageSize)
                .ToList();

            return BuildPagination(query, null, page, pageSize, itemsPaged);
        }
        #endregion Paginate

        #region Paginate Async
        public static async Task<Pagination<T>> ToNullablePaginateAsync<T>(
            this IQueryable<T> query,
            Expression<Func<T, bool>>? predicate,
            int page,
            int pageSize)
        {
            var pagination = predicate != null
                ? await ToPaginateAsync(query, predicate, page, pageSize).ConfigureAwait(false)
                : await ToPaginateAsync(query, page, pageSize).ConfigureAwait(false);

            return pagination;
        }

        public static async Task<Pagination<T>> ToPaginateAsync<T>(
            this IQueryable<T> query,
            Expression<Func<T, bool>> predicate,
            int page,
            int pageSize)
        {
            var entities = await query
                .Where(predicate)
                .Paginate(page, pageSize)
                .ToListAsync()
                .ConfigureAwait(false);

            return await BuildPaginationAsync(query, predicate, page, pageSize, entities).ConfigureAwait(false);
        }

        private static async Task<Pagination<T>> BuildPaginationAsync<T>(IQueryable<T> query, Expression<Func<T, bool>>? predicate, int page, int pageSize, List<T> entities)
        {
            var totalItems = await query
                .NullableCountAsync(predicate)
                .ConfigureAwait(false);

            double itemsPerPage = pageSize <= 0
                ? totalItems
                : pageSize;

            var totalPages = (long)Math.Ceiling(totalItems / itemsPerPage);

            return new Pagination<T>(
                entities,
                totalItems,
                totalPages,
                page,
                pageSize);
        }

        public static async Task<Pagination<T>> ToPaginateAsync<T>(
            this IQueryable<T> query,
            int page,
            int pageSize)
        {
            var entities = await query
                .Paginate(page, pageSize)
                .ToListAsync()
                .ConfigureAwait(false);

            return await BuildPaginationAsync(query, null, page, pageSize, entities).ConfigureAwait(false);
        }
        #endregion Paginate Async
    }
}
