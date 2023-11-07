using System.Linq.Expressions;

namespace APICacheWithRedis.Utils
{
    public static class PaginationQuery
    {
        public static IQueryable<T> CustomQuery<T>(this IQueryable<T> query, Expression<Func<T, bool>>? filter = null) where T : class
        {
            if (filter != null)
            {
                query = query.Where(filter);
            }

            return query;
        }

        public static IQueryable<T> CustomPagination<T>(this IQueryable<T> query, int page = 0, int pageSize = 10)
        {
            query = query.Skip((page == 0 ? 0 : page) * pageSize);
            query = query.Take((int)pageSize);
            return query;
        }
    }
}