using CQ.UnitOfWork.Abstractions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace CQ.UnitOfWork.EfCore.Extensions
{
    public static class PaginateExtension
    {
        public static IQueryable<T> Paginate<T>(this IQueryable<T> elements, int page = 1, int pageSize = 10)
        {
            if(page <= 0 || pageSize <= 0)
            {
                return elements;
            }

            var skipTo = (page - 1) * pageSize;

            return elements.Skip(skipTo).Take(pageSize);
        }

        public static async Task<Pagination<T>> PaginateAsync<T>(
            this IQueryable<T> elements,
            Expression<Func<T, bool>>? predicate = null,
            int page = 1,
            int pageSize = 10)
        {
            var entities = await elements
                .NullableWhere(predicate)
                .Paginate(page, pageSize)
                .ToListAsync()
                .ConfigureAwait(false);

            var totalItems = await elements
                .NullableCountAsync(predicate)
                .ConfigureAwait(false);

            double itemsPerPage = pageSize == 0 ? totalItems : pageSize;
            var totalPages = Convert.ToInt64(Math.Ceiling(totalItems / itemsPerPage));

            return new Pagination<T>(
                entities,
                totalItems,
                totalPages);
        }

        public static Pagination<T> Paginate<T>(
            this IQueryable<T> elements,
            Expression<Func<T, bool>>? predicate = null,
            int page = 1,
            int pageSize = 10)
        {
            var itemsPaged = elements
                .NullableWhere(predicate)
                .Paginate(page, pageSize)
                .ToList();

            var totalItems = elements
                .NullableCount(predicate);

            double itemsPerPage = pageSize == 0 ? totalItems : pageSize;
            var totalPages = Convert.ToInt64(Math.Ceiling(totalItems / itemsPerPage));

            return new Pagination<T>(
                itemsPaged,
                totalItems,
                totalPages);
        }
    }
}
