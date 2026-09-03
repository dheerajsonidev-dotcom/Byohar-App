using Byohar.Application.Exceptions;
using Byohar.Application.Specifications.Base;
using Byohar.Domain.Common;
using Byohar.Shared.Wrapper;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

namespace Byohar.Infrastructure.Extensions
{
    public static class QueryableExtensions
    {
        public static async Task<PaginatedResult<T>> ToPaginatedListAsync<T>(this IQueryable<T> source, int pageNumber, int pageSize) where T : class
        {
            if (source == null) throw new ApiException();
            pageSize = pageSize == 0 ? int.MaxValue : pageSize;
            int count = await source.CountAsync();
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            List<T> items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return PaginatedResult<T>.Success(items, count, pageNumber, pageSize);
        }

        public static PaginatedResult<T> ToPaginatedEnumerableList<T>(this IEnumerable<T> source, int pageNumber, int pageSize) where T : class
        {
            if (source == null) throw new ApiException();
            pageSize = pageSize == 0 ? int.MaxValue : pageSize;
            int count = source.Count();
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            List<T> items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return PaginatedResult<T>.Success(items, count, pageNumber, pageSize);
        }

        public static IQueryable<T> Specify<T>(this IQueryable<T> query, ISpecification<T> spec) where T : class, IEntity
        {
            var queryableResultWithIncludes = spec.Includes
                .Aggregate(query, (current, include) => current.Include(include));

            var secondaryResult = spec.IncludeStrings
                .Aggregate(queryableResultWithIncludes,
                    (current, include) => current.Include(include));

            if (spec.Criteria != null)
            {
                return secondaryResult.Where(spec.Criteria);
            }

            return secondaryResult;
        }


        public static IEnumerable<T> Specify<T>(this IEnumerable<T> collection, ISpecification<T> spec) where T : class, IEntity
        {
            if (spec.Criteria != null)
            {
                collection = collection.Where(spec.Criteria.Compile()).ToList();
            }

            return collection;
        }


        public static IEnumerable<T> SelectRecursive<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> selector)
        {
            foreach (var parent in source)
            {
                yield return parent;

                var children = selector(parent);
                foreach (var child in children.SelectRecursive(selector))
                    yield return child;
            }
        }

        public static IQueryable<T> ApplySorting<T>(this IQueryable<T> query, string sortBy, string sortOrder)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(T), "x");

            MemberExpression property;

            if (sortBy.Contains('.', StringComparison.CurrentCulture))
            {
                string[] parts = sortBy.Split('.');

                sortBy = parts[0];
                string sortProperty = parts[1];

                property = Expression.Property(parameter, sortBy);
                property = Expression.PropertyOrField(property, sortProperty);
            }
            else
            {
                property = Expression.Property(parameter, sortBy);
            }

            LambdaExpression lambda = Expression.Lambda(property, parameter);

            string methodName = sortOrder == "Desc" ? "OrderByDescending" : "OrderBy";

            MethodCallExpression methodCall = Expression.Call(
                typeof(Queryable),
                methodName,
                new[] { typeof(T), property.Type },
                query.Expression,
                Expression.Quote(lambda)
            );

            return query.Provider.CreateQuery<T>(methodCall);
        }

        public static IQueryable<TSource> WhereIf<TSource>(
          this IQueryable<TSource> source,
          bool condition,
          Expression<Func<TSource, bool>> predicate)
        {
            if (condition)
            {
                return source.Where(predicate);
            }

            return source;
        }

        public static IEnumerable<TSource> WhereIf<TSource>(
             this IEnumerable<TSource> source,
             bool condition,
             Func<TSource, bool> predicate)
        {
            if(source == null || source?.Count() == 0)
            {
                return source;
            }

            if (condition)
            {
                return source.Where(predicate);
            }

            return source;
        }


        public static List<T> ApplySortingToList<T>(this List<T> list, string sortBy, string sortOrder)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            if (string.IsNullOrEmpty(sortBy))
            {
                return list;
            }

            var property = typeof(T).GetProperty(sortBy, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (property == null)
            {
                throw new ArgumentException($"Property '{sortBy}' not found on type '{typeof(T).Name}'.");
            }

            list.Sort((x, y) =>
            {
                var xValue = property.GetValue(x);
                var yValue = property.GetValue(y);

                if (xValue == null && yValue == null)
                {
                    return 0;
                }
                if (xValue == null)
                {
                    return sortOrder == "Desc" ? 1 : -1;
                }
                if (yValue == null)
                {
                    return sortOrder == "Desc" ? -1 : 1;
                }

                return sortOrder == "Desc"
                    ? Comparer.DefaultInvariant.Compare(yValue, xValue)
                    : Comparer.DefaultInvariant.Compare(xValue, yValue);
            });

            return list;
        }

    }
}