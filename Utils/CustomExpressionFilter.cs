using System.Linq.Expressions;
using APICacheWithRedis.Payload.Filter;

namespace APICacheWithRedis.Utils{
    public static class CustomExpressionFilter<T> where T : class
    {
        public class ExpressionFilter
        {
            public required string ColumnName { get; set; }
            public required string Value { get; set; }
        }
        public static Expression<Func<T, bool>>? CustomFilter(List<ColumnFilter> columnFilters, string className)
        {
            Expression<Func<T, bool>>? filters = null;
            try
            {
                var expressionFilters = new List<ExpressionFilter>();
                foreach (var item in columnFilters)
                {
                    if(item.Id != null && item.Value != null)                    
                        expressionFilters.Add(new ExpressionFilter() { ColumnName = item.Id, Value = item.Value });
                }
                // Create the parameter expression for the input data
                var parameter = Expression.Parameter(typeof(T), className);

                // Build the filter expression dynamically
                Expression? filterExpression = null;
                foreach (var filter in expressionFilters)
                {
                    if(filter.ColumnName != null){
                        var property = Expression.Property(parameter, filter.ColumnName);

                        Expression comparison;

                        if (property.Type == typeof(string))
                        {
                            var constant = Expression.Constant(filter.Value);
                            comparison = Expression.Call(property, "Contains", Type.EmptyTypes, constant);
                        }
                        else if (property.Type == typeof(double))
                        {
                            var constant = Expression.Constant(Convert.ToDouble(filter.Value));
                            comparison = Expression.Equal(property, constant);
                        }
                        else if (property.Type == typeof(Guid))
                        {
                            var constant = Expression.Constant(Guid.Parse(filter.Value));
                            comparison = Expression.Equal(property, constant);
                        }
                        else
                        {
                            var constant = Expression.Constant(Convert.ToInt32(filter.Value));
                            comparison = Expression.Equal(property, constant);
                        }


                        filterExpression = filterExpression == null
                            ? comparison
                            : Expression.And(filterExpression, comparison);
                    }
                }

                // Create the lambda expression with the parameter and the filter expression
                filters = filterExpression == null ? null : Expression.Lambda<Func<T, bool>>(filterExpression, parameter);
            }
            catch (Exception)
            {
                filters = null;
            }
            return filters;
        }

    }
}