namespace DomainServices
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Text;
    using Ardalis.GuardClauses;
    using Newtonsoft.Json;

    /// <summary>
    ///     Class QueryCondition.
    /// </summary>
    [Serializable]
    public class QueryCondition
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="QueryCondition" /> class.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="queryOperator">The query operator.</param>
        /// <param name="value">The value.</param>
        [JsonConstructor]
        public QueryCondition(string item, QueryOperator queryOperator, object value)
        {
            Guard.Against.NullOrEmpty(item, nameof(item));
            if (queryOperator == QueryOperator.Any && !(value is ICollection))
            {
                throw new ArgumentException($"Value '{value}' is not a collection. When QueryOperator is ANY, the value must be a collection.", nameof(value));
            }

            Item = item;
            QueryOperator = queryOperator;
            Value = value is long ? Convert.ToInt32(value) : value;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="QueryCondition" /> class.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="value">The value.</param>
        public QueryCondition(string item, object value)
            : this(item, QueryOperator.Equal, value)
        {
        }

        /// <summary>
        ///     Gets the item.
        /// </summary>
        public string Item { get; }

        /// <summary>
        ///     Gets the query operator.
        /// </summary>
        public QueryOperator QueryOperator { get; }

        /// <summary>
        ///     Gets the value.
        /// </summary>
        public object? Value { get; }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        public override string ToString()
        {
            var values = Value ?? "null";
            if (values.GetType().IsCollection())
            {
                var sb = new StringBuilder("(");
                foreach (var v in (IEnumerable)values)
                {
                    if (sb.Length > 1)
                    {
                        sb.Append(" OR ");
                    }

                    sb.Append(v);
                }

                sb.Append(')');
                values = sb.ToString();
            }

            return string.Format(CultureInfo.InvariantCulture, "{0} {1} {2}", Item, QueryOperator.GetDescription(), values);
        }
    }
}