using HCQS.BackEnd.Common.Dto.BaseRequest;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

namespace HCQS.BackEnd.DAL.Util
{
    public class DataPresentationHelper
    {
      
        public static List<T> ApplyPaging<T>(IEnumerable<T> source, int pageIndex, int pageSize)
        {
            int toSkip = (pageIndex - 1) * pageSize;
            return source.Skip(toSkip).Take(pageSize).ToList();
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

        public static List<T> ApplyFiltering<T>(List<T> source, IList<FilterInfo> filterList)
        {
            if (source == null || source.Count == 0 || filterList == null || filterList.Count == 0)
            {
                return source;
            }

            // Initialize the combined expression to always true
            Expression<Func<T, bool>> combinedExpression = t => true;

            foreach (var filterInfo in filterList)
            {
                Expression<Func<T, bool>> subFilter = CreateFilterExpression<T>(filterInfo);

                if (subFilter != null)
                {
                    // Combine the sub-filter with the existing combined expression using AndAlso
                    combinedExpression = Expression.Lambda<Func<T, bool>>(
                        Expression.AndAlso(
                            Expression.Invoke(subFilter, combinedExpression.Parameters),
                            combinedExpression.Body),
                        combinedExpression.Parameters);
                }
            }

            // Apply the combined filter to the source and return the result
            return source.Where(combinedExpression.Compile()).ToList();
        }

        private static Expression<Func<T, bool>> CreateFilterExpression<T>(FilterInfo filterInfo)
        {
            var parameter = Expression.Parameter(typeof(T));
            var property = Expression.Property(parameter, filterInfo.fieldName);

            if (filterInfo.min.HasValue && filterInfo.max.HasValue)
            {
                // Range filter
                var lowerBound = Expression.Constant(filterInfo.min.Value, property.Type);
                var upperBound = Expression.Constant(filterInfo.max.Value, property.Type);
                var lowerCondition = Expression.GreaterThanOrEqual(property, lowerBound);
                var upperCondition = Expression.LessThanOrEqual(property, upperBound);
                var rangeCondition = Expression.AndAlso(lowerCondition, upperCondition);
                return Expression.Lambda<Func<T, bool>>(rangeCondition, parameter);
            }
            else if (filterInfo.min.HasValue)
            {
                // Greater than or equal filter
                var lowerBound = Expression.Constant(filterInfo.min.Value, property.Type);
                var condition = Expression.GreaterThanOrEqual(property, lowerBound);
                return Expression.Lambda<Func<T, bool>>(condition, parameter);
            }
            else if (filterInfo.max.HasValue)
            {
                // Less than or equal filter
                var upperBound = Expression.Constant(filterInfo.max.Value, property.Type);
                var condition = Expression.LessThanOrEqual(property, upperBound);
                return Expression.Lambda<Func<T, bool>>(condition, parameter);
            }

            // Add more filter types based on your requirements...

            return null; // Return null if unable to create a valid filter expression
        }

        public static List<T> ApplySorting<T>(List<T> filteredData, IList<SortInfo> sortingList)
        {
            // Order by a constant to initiate sorting.
            IOrderedEnumerable<T> orderedQuery = filteredData.OrderBy(x => 0);

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
                    orderedQuery = orderedQuery.ThenBy(lambda.Compile());
                }
                else
                {
                    orderedQuery = orderedQuery.ThenByDescending(lambda.Compile());
                }
            }

            return orderedQuery.ToList();
        }

        public static int CalculateTotalPageSize(int totalRecord, int pageSize)
        {
            return (int)Math.Ceiling(totalRecord * 1.00 / pageSize);
        }
    }
}