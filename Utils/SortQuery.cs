using System.Linq.Expressions;
using APICacheWithRedis.Payload.Sort;

namespace APICacheWithRedis.Utils{
    public static class SortQuery{
        public static IQueryable<T> SortBy<T>(this IQueryable<T> source, List<ColumnSorting> columnSortings)
        {
            Expression expression = source.Expression;
            bool firstTime = true;

            foreach (var f in columnSortings)
            {
                if(f.Id == null) break;
                // { x }
                var parameter = Expression.Parameter(typeof(T), "x");

                // { x.FIELD }, e.g, { x.ID }, { x.Name }, etc
                var selector = Expression.PropertyOrField(parameter, f.Id);

                // { x => x.FIELD }
                var lambda = Expression.Lambda(selector, parameter);

                // You can include sorting directions for advanced cases
                var method = firstTime
                    ? !f.Desc ? "OrderBy" : "OrderByDescending" 
                    : !f.Desc ?"ThenBy" : "ThenByDescending";

                // { OrderBy(x => x.FIELD) }
                expression = Expression.Call(
                    typeof(Queryable), 
                    method,
                    new Type[] { source.ElementType, selector.Type },
                    expression, 
                    Expression.Quote(lambda)
                );

                firstTime = false;
            }

            return firstTime 
                ? source
                : source.Provider.CreateQuery<T>(expression);
        }
    }
}