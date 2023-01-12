namespace DomainServices;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Ardalis.GuardClauses;

/// <summary>
///     Class representing a query.
/// </summary>
public class Query<T> : IEnumerable<QueryCondition>
{
    private readonly List<QueryCondition> _queryConditions;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Query{T}" /> class.
    /// </summary>
    public Query()
    {
        _queryConditions = new List<QueryCondition>();
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Query{T}" /> class.
    /// </summary>
    /// <param name="queryCondition">A query condition.</param>
    public Query(QueryCondition queryCondition)
        : this()
    {
        Guard.Against.Null(queryCondition, nameof(queryCondition));
        Validate(queryCondition);
        _queryConditions.Add(queryCondition);
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Query{T}" /> class.
    /// </summary>
    /// <param name="queryConditions">The query conditions.</param>
    public Query(IEnumerable<QueryCondition> queryConditions) : this()
    {
        var conditions = queryConditions as QueryCondition[] ?? queryConditions.ToArray();
        Guard.Against.NullOrEmpty(conditions, nameof(queryConditions));
        foreach (var condition in conditions)
        {
            Validate(condition);
        }

        _queryConditions.AddRange(conditions);
    }

    /// <summary>
    ///     Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>
    ///     A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
    /// </returns>
    public IEnumerator<QueryCondition> GetEnumerator()
    {
        return _queryConditions.GetEnumerator();
    }

    /// <summary>
    ///     Returns an enumerator that iterates through a collection.
    /// </summary>
    /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    ///     Adds the specified query condition.
    /// </summary>
    /// <param name="queryCondition">The query condition.</param>
    public void Add(QueryCondition queryCondition)
    {
        Validate(queryCondition);
        _queryConditions.Add(queryCondition);
    }

    /// <summary>
    ///     Converts the query into a LINQ expression.
    /// </summary>
    /// <returns>Expression&lt;Func&lt;T, System.Boolean&gt;&gt;.</returns>
    public Expression<Func<T, bool>> ToExpression()
    {
        return ExpressionBuilder.Build<T>(_queryConditions);
    }

    /// <summary>
    ///     Returns a <see cref="string" /> that represents this instance.
    /// </summary>
    /// <returns>A <see cref="string" /> that represents this instance.</returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        foreach (var condition in _queryConditions)
        {
            if (sb.Length > 0)
            {
                sb.Append(" AND ");
            }

            sb.Append(condition);
        }

        return sb.ToString();
    }

    private static void Validate(QueryCondition condition)
    {
        var properties = typeof(T).GetProperties();
        var propertyNames = properties.Select(p => p.Name).ToArray();
        if (!propertyNames.Contains(condition.Item) || condition.Value is null)
        {
            return;
        }

        var valueType = condition.Value.GetType();
        var propertyType = properties.Single(p => p.Name == condition.Item).PropertyType;
        if (valueType.IsCollection())
        {
            foreach (var value in (ICollection)condition.Value)
            {
                var elementType = value.GetType();
                if (!propertyType.IsAssignableFrom(elementType))
                {
                    throw new Exception($"The type of the value '{elementType}' is wrong for condition item '{condition.Item}'. The value must be assignable to the type '{propertyType}'");
                }
            }
        }
        else if (!propertyType.IsAssignableFrom(valueType))
        {
            throw new Exception($"The type of the value '{valueType}' is wrong for condition item '{condition.Item}'. The value must be assignable to the type '{propertyType}'");
        }
    }
}