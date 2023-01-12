namespace DomainServices.Logging;

using System;

/// <summary>
///     Logger abstraction
/// </summary>
public interface ILogger
{
    /// <summary>
    ///     Adds the specified entry to the log.
    /// </summary>
    /// <param name="logEntry">The log entry.</param>
    void Log(LogEntry logEntry);

    /// <summary>
    ///     Occurs when a new entry is added.
    /// </summary>
    event EventHandler<EventArgs<LogEntry>> EntryAdded;
}