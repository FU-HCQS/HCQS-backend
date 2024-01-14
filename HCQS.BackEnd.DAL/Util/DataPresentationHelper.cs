using HCQS.BackEnd.Common.Dto.BaseRequest;
using System.Linq.Expressions;

namespace HCQS.BackEnd.DAL.Util
{
    public class DataPresentationHelper
    {
        public static IOrderedQueryable<T> ApplyFiltering<T>(IOrderedQueryable<T> source, IList<FilterInfo> filterList)
        {
            if (source == null || source.Count() == 0)
            {
                return source;
            }
            Expression<Func<T, bool>> combinedExpression = t => true;
            if (filterList != null)
            {
                foreach (var filterInfo in filterList)
                {
                    Expression<Func<T, bool>> subFilter = null;
                    /*if (!filterInfo.isValueFilter)
                        subFilter = CreateRangeFilterExpression<T>(filterInfo);
                    else */
                    subFilter = CreateRangeFilterExpression<T>(filterInfo);

                    if (subFilter != null)
                    {
                        var invokedExpr = Expression.Invoke(subFilter, combinedExpression.Parameters);
                        combinedExpression = Expression.Lambda<Func<T, bool>>(
                            Expression.AndAlso(invokedExpr, combinedExpression.Body),
                            combinedExpression.Parameters);
                    }
                }
                IOrderedQueryable<T> result = (IOrderedQueryable<T>)source.Where(combinedExpression);
                return result;
            }
            return source;
        }

        public static IOrderedQueryable<T> ApplyPaging<T>(IOrderedQueryable<T> source, int pageIndex, int pageSize)
        {
            int toSkip = (pageIndex - 1) * pageSize;
            return (IOrderedQueryable<T>)source.Skip(toSkip).Take(pageSize);
        }

        private static Expression<Func<T, bool>> CreateRangeFilterExpression<T>(FilterInfo filterInfoToRange)
        {
            var parameter = Expression.Parameter(typeof(T), "c");
            var property = Expression.PropertyOrField(parameter, filterInfoToRange.fieldName);
            var minValue = Expression.Constant(filterInfoToRange.min, typeof(double?));
            var maxValue = Expression.Constant(filterInfoToRange.max, typeof(double?));

            var greaterThanOrEqual = Expression.GreaterThanOrEqual(property, minValue);
            var lessThanOrEqual = Expression.LessThanOrEqual(property, maxValue);
            var andAlso = Expression.AndAlso(greaterThanOrEqual, lessThanOrEqual);

            return Expression.Lambda<Func<T, bool>>(andAlso, parameter);
        }

        /*private static Expression<Func<T, bool>> CreateValueFilterExpression<T>(FilterInfo filterInfoToValue)
        {
            var parameter = Expression.Parameter(typeof(T), "c");
            var conjunctions = new List<Expression>();

            var fieldName = filterInfoToValue.fieldName;
            var filterValues = filterInfoToValue.values;

            if (filterValues is IEnumerable<object> values)
            {
                // Create equality expressions for each value
                foreach (var filterValue in values)
                {
                    var property = Expression.Property(parameter, fieldName);
                    var value = Expression.Constant(filterValue);
                        var equality = Expression.Equal(property, value);
                    conjunctions.Add(equality);
                }
            }
            else
            {
                throw new InvalidOperationException("filterValues must be of type IEnumerable<object> for filtering.");
            }
            // Use OR for the same field name and AND for different field names
            var combinedFilter = conjunctions.Aggregate((current, next) => Expression.Or(current, next));
            return Expression.Lambda<Func<T, bool>>(combinedFilter, parameter);
        }*/

        public static IOrderedQueryable<T> ApplySorting<T>(IOrderedQueryable<T> filteredData, IList<SortInfo> sortingList)
        {
            IOrderedQueryable<T> orderedQuery = filteredData;

            orderedQuery = filteredData.OrderBy(x => 0); // Order by a constant to initiate sorting.

            foreach (var sortInfo in sortingList)
            {
                var property = typeof(T).GetProperty(sortInfo.fieldName);

                if (property == null)
                {
                    throw new ArgumentException($"Property '{sortInfo.fieldName}' not found in type '{typeof(T).FullName}'.");
                }

                // Create an expression to represent property access
                var parameter = Expression.Parameter(typeof(T));
                var propertyAccess = Expression.Property(parameter, property);
                var lambda = Expression.Lambda<Func<T, object>>(Expression.Convert(propertyAccess, typeof(object)), parameter);

                if (sortInfo.ascending)
                {
                    orderedQuery = orderedQuery.ThenBy(lambda);
                }
                else
                {
                    orderedQuery = orderedQuery.ThenByDescending(lambda);
                }
            }

            return orderedQuery;
        }

        public static int CalculateTotalPageSize(int totalRecord, int pageSize)
        {
            return (int)Math.Ceiling(totalRecord * 1.00 / pageSize);
        }
    }
}