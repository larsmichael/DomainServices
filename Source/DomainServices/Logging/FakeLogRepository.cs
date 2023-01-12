namespace DomainServices.Logging;

using System;
using System.Collections.Generic;
using System.Linq;

public class FakeLogRepository : ILogRepository
{
    private readonly List<LogEntry> _logEntries;

    public FakeLogRepository()
    {
        _logEntries = new List<LogEntry>();
    }

    public FakeLogRepository(IEnumerable<LogEntry> logEntries)
        : this()
    {
        foreach (var logEntry in logEntries)
        {
            _logEntries.Add(logEntry);
        }
    }

    public event EventHandler<EventArgs<LogEntry>>? EntryAdded;

    public IEnumerable<LogEntry> Get(IEnumerable<QueryCondition> query)
    {
        var predicate = ExpressionBuilder.Build<LogEntry>(query);
        return _logEntries.AsQueryable().Where(predicate).ToArray();
    }

    public Maybe<LogEntry> Last(IEnumerable<QueryCondition> query)
    {
        var predicate = ExpressionBuilder.Build<LogEntry>(query);
        return _logEntries.AsQueryable().Where(predicate).Last().ToMaybe();
    }

    public void Log(LogEntry logEntry)
    {
        _logEntries.Add(logEntry);
        OnAdded(logEntry);
    }

    protected virtual void OnAdded(LogEntry logEntry)
    {
        EntryAdded?.Invoke(this, new EventArgs<LogEntry>(logEntry));
    }
}