namespace DomainServices
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Text;

    /// <summary>
    ///     Various generic extension methods.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        ///     Gets the description (symbology) of the query operator.
        /// </summary>
        /// <param name="queryOperator">The query operator.</param>
        /// <returns>System.String.</returns>
        public static string GetDescription(this QueryOperator queryOperator)
        {
            var attributes = (DescriptionAttribute[])queryOperator.GetType().GetField(queryOperator.ToString())!.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }

        /// <summary>
        ///     Converts the query to a command string.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>System.String.</returns>
        public static string ToCommandString(this IEnumerable<QueryCondition> query)
        {
            var sb = new StringBuilder();
            foreach (var condition in query)
            {
                if (sb.Length > 0)
                {
                    sb.Append(" AND ");
                }

                sb.Append($"({condition.Item} {condition.QueryOperator.GetDescription()} ?)");
            }

            return sb.ToString();
        }

        public static bool IsCollection(this Type type)
        {
            return typeof(ICollection).IsAssignableFrom(type);
        }
    }
}