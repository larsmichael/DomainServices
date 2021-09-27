namespace DomainServices
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Dynamic;
    using System.Linq;
    using System.Text;
    using Newtonsoft.Json.Linq;

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
            var attributes = (DescriptionAttribute[])queryOperator.GetType().GetField(queryOperator.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
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

        /// <summary>
        ///     Converts the object array to a DataTable object.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>DataTable.</returns>
        public static DataTable ToDataTable(this object[,] data)
        {
            var rowCount = data.GetLength(0);
            var columnCount = data.GetLength(1);
            var dataTable = new DataTable();
            for (var i = 0; i < columnCount; i++)
            {
                dataTable.Columns.Add();
            }

            foreach (var row in Enumerable.Range(1, rowCount))
            {
                dataTable.Rows.Add(Enumerable.Range(1, columnCount).Select(col => data[row - 1, col - 1]).ToArray());
            }

            return dataTable;
        }

        /// <summary>
        ///     Converts an object to a dynamic object.
        /// </summary>
        /// <param name="value">The object to convert.</param>
        /// <returns>dynamic.</returns>
        public static dynamic ToDynamic(this object value)
        {
            IDictionary<string, object> expando = new ExpandoObject();
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(value.GetType()))
            {
                expando.Add(property.Name, property.GetValue(value));
            }

            return (ExpandoObject)expando;
        }

        public static bool IsCollection(this Type type)
        {
            return typeof(ICollection).IsAssignableFrom(type);
        }

        public static JObject Filter(this JObject obj, params string[] selects)
        {
            var result = (JContainer)new JObject();

            foreach (var select in selects)
            {
                var token = obj.SelectToken(select);
                if (token == null)
                    continue;

                result.Merge(GetNewParent(token.Parent));
            }

            return (JObject)result;
        }

        private static JToken GetNewParent(JToken token)
        {
            var result = new JObject(token);

            var parent = token;
            while ((parent = parent.Parent) != null)
            {
                if (parent is JProperty property)
                    result = new JObject(new JProperty(property.Name, result));
            }

            return result;
        }
        
    }
}