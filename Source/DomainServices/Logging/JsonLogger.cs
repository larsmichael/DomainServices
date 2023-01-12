namespace DomainServices.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using Repositories;

/// <summary>
///     Class for logging to a JSON file.
/// </summary>
public class JsonLogger : JsonRepository<LogEntry, Guid>, ILogRepository
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="JsonLogger" /> class.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    public JsonLogger(string filePath) : base(filePath)
    {
    }

    /// <summary>
    ///     Adds the specified entry to the log.
    /// </summary>
    /// <param name="logEntry">The log entry.</param>
    public void Log(LogEntry logEntry)
    {
        Add(logEntry);
        OnAdded(logEntry);
    }

    /// <summary>
    ///     Queries the log using a list of query conditions.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <returns>IEnumerable&lt;LogEntry&gt;.</returns>
    public IEnumerable<LogEntry> Get(IEnumerable<QueryCondition> query)
    {
        var predicate = ExpressionBuilder.Build<LogEntry>(query);
        return Get(predicate);
    }

    /// <summary>
    ///     Queries the last log entry using a list of query conditions.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <returns>IEnumerable&lt;LogEntry&gt;.</returns>
    public Maybe<LogEntry> Last(IEnumerable<QueryCondition> query)
    {
        return Count() == 0 ? Maybe.Empty<LogEntry>() : Get(query).OrderByDescending(logEntry => logEntry.DateTime).FirstOrDefault().ToMaybe();
    }

    /// <summary>
    ///     Occurs when a new log entry is added.
    /// </summary>
    public event EventHandler<EventArgs<LogEntry>>? EntryAdded;

    private void OnAdded(LogEntry logEntry)
    {
        EntryAdded?.Invoke(this, new EventArgs<LogEntry>(logEntry));
    }
}