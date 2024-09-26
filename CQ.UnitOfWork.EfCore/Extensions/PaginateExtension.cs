using CQ.UnitOfWork.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CQ.UnitOfWork.EfCore.Extensions
{
    public static class PaginateExtension
    {
        public static IQueryable<T> Paginate<T>(
            this IQueryable<T> query,
            int page = 1,
            int pageSize = 10)
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

        public static Pagination<T> Paginate<T>(
            this IQueryable<T> query,
            Expression<Func<T, bool>>? predicate = null,
            int page = 1,
            int pageSize = 10)
        {
            var itemsPaged = query
                .NullableWhere(predicate)
                .Paginate(page, pageSize)
                .ToList();

            var totalItems = query
                .NullableCount(predicate);

            double itemsPerPage = pageSize <= 0
                ? totalItems
                : pageSize;

            var totalPages = Convert.ToInt64(Math.Ceiling(totalItems / itemsPerPage));

            return new Pagination<T>(
                itemsPaged,
                totalItems,
                totalPages);
        }

        public static async Task<Pagination<T>> PaginateAsync<T>(
            this IQueryable<T> query,
            Expression<Func<T, bool>>? predicate = null,
            int page = 1,
            int pageSize = 10)
        {
            var entities = await query
                .NullableWhere(predicate)
                .Paginate(page, pageSize)
                .ToListAsync()
                .ConfigureAwait(false);

            var totalItems = await query
                .NullableCountAsync(predicate)
                .ConfigureAwait(false);

            double itemsPerPage = pageSize <= 0
                ? totalItems
                : pageSize;
            
            var totalPages = Convert.ToInt64(Math.Ceiling(totalItems / itemsPerPage));

            return new Pagination<T>(
                entities,
                totalItems,
                totalPages,
                page,
                pageSize);
        }
    }
}
