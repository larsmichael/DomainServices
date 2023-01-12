#nullable enable
namespace DomainServices.Logging;

using System;
using System.IO;
using Ardalis.GuardClauses;

/// <summary>
///     Simple Logger.
/// </summary>
public class SimpleLogger : ILogger
{
    private readonly string _filePath;

    /// <summary>
    ///     Initializes a new instance of the <see cref="SimpleLogger" /> class.
    /// </summary>
    /// <param name="filePath">The filePath.</param>
    public SimpleLogger(string filePath)
    {
        Guard.Against.NullOrEmpty(filePath, nameof(filePath));
        var directory = Path.GetDirectoryName(filePath);
        if (directory is not null && directory != "" && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        _filePath = filePath;
    }

    /// <summary>
    ///     Adds the specified entry to the log.
    /// </summary>
    /// <param name="logEntry">The log entry.</param>
    public void Log(LogEntry logEntry)
    {
        var entry = $"{logEntry.DateTime:yyyy-MM-dd HH:mm:ss} {logEntry.Source} [{logEntry.LogLevel}] - {logEntry.Text}{Environment.NewLine}";

        try
        {
            File.AppendAllText(_filePath, entry);
        }
        catch
        {
            // do nothing, logging failure shouldn't crash the application
        }

        EntryAdded?.Invoke(this, new EventArgs<LogEntry>(logEntry));
    }

    /// <summary>
    ///     Occurs when a new entry is added.
    /// </summary>
    public event EventHandler<EventArgs<LogEntry>>? EntryAdded;
}