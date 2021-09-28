namespace DomainServices
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Ardalis.GuardClauses;

    /// <summary>
    ///     Class ExpressionBuilder.
    /// </summary>
    public static class ExpressionBuilder
    {
        /// <summary>
        ///     Builds a LINQ expression from a collection of query conditions.
        /// </summary>
        /// <typeparam name="T">Type of LINQ parameter</typeparam>
        /// <param name="filter">The collection of query conditions.</param>
        /// <returns>Expression&lt;Func&lt;T, System.Boolean&gt;&gt;.</returns>
        /// <exception cref="ArgumentNullException">filter</exception>
        /// <exception cref="ArgumentException">Filter cannot be empty.;filter</exception>
        public static Expression<Func<T, bool>> Build<T>(IEnumerable<QueryCondition> filter)
        {
            Guard.Against.Null(filter, nameof(filter));
            var queryConditions = filter as IList<QueryCondition> ?? filter.ToList();
            if (!queryConditions.Any())
            {
                throw new ArgumentException("Filter cannot be empty.", nameof(filter));
            }

            var parameter = Expression.Parameter(typeof(T), "t");
            Expression? expression = null;
            foreach (var condition in queryConditions)
            {
                expression = expression == null ? ToExpression(parameter, condition) : Expression.AndAlso(expression, ToExpression(parameter, condition));
            }

            return Expression.Lambda<Func<T, bool>>(expression!, parameter);
        }

        private static Expression ToExpression(ParameterExpression param, QueryCondition condition)
        {
            Expression property;
            Expression value;
            if (condition.Value is not null && condition.Value.GetType().IsEnum)
            {
                var enumType = Enum.GetUnderlyingType(condition.Value.GetType());
                property = Expression.Convert(Expression.Property(param, condition.Item), enumType);
                value = Expression.Convert(Expression.Constant(condition.Value), enumType);
            }
            else
            {
                property = Expression.Property(param, condition.Item);
                value = Expression.Constant(condition.Value);
            }

            switch (condition.QueryOperator)
            {
                case QueryOperator.GreaterThan:
                    return Expression.GreaterThan(property, value);

                case QueryOperator.GreaterThanOrEqual:
                    return Expression.GreaterThanOrEqual(property, value);

                case QueryOperator.LessThan:
                    return Expression.LessThan(property, value);

                case QueryOperator.LessThanOrEqual:
                    return Expression.LessThanOrEqual(property, value);

                case QueryOperator.Equal:
                    return Expression.Equal(property, value);

                case QueryOperator.NotEqual:
                    return Expression.NotEqual(property, value);

                default:
                    throw new NotImplementedException($"Query operator '{condition.QueryOperator}' is not supported.");
            }
        }
    }
}