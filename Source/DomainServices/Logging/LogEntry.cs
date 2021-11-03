namespace DomainServices.Logging
{
    using System.Text.Json.Serialization;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Ardalis.GuardClauses;
    using Abstractions;

    /// <summary>
    ///     An immutable structure representing a log entry
    /// </summary>
    public readonly struct LogEntry : IComparable<LogEntry>, IEntity<Guid>
    {
        private readonly IDictionary<string, object> _metadata;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogEntry" /> struct.
        /// </summary>
        /// <param name="id">The unique identifier</param>
        /// <param name="logLevel">The log level.</param>
        /// <param name="text">The text message.</param>
        /// <param name="source">The source.</param>
        /// <param name="tag">A tag.</param>
        /// <param name="machineName">The machine name.</param>
        /// <param name="dateTime">The date time.</param>
        /// <param name="metadata">Metadata.</param>
        [JsonConstructor]
        public LogEntry(Guid id, LogLevel logLevel, string text, string source, string? tag = null, string? machineName = null, DateTime dateTime = default, IDictionary<string, object>? metadata = null)
            : this()
        {
            Guard.Against.NullOrEmpty(text, nameof(text));
            Guard.Against.NullOrEmpty(source, nameof(source));
            Id = id;
            LogLevel = logLevel;
            Text = text;
            Source = source;
            Tag = tag;
            DateTime = dateTime == default ? DateTime.Now : dateTime;
            MachineName = machineName ?? Environment.GetEnvironmentVariable("COMPUTERNAME");
            _metadata = metadata ?? new Dictionary<string, object>();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogEntry" /> struct.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="text">The text message.</param>
        /// <param name="source">The source.</param>
        /// <param name="tag">A tag.</param>
        /// <param name="machineName">The machine name.</param>
        /// <param name="dateTime">The date time.</param>
        public LogEntry(LogLevel logLevel, string text, string source, string? tag = null, string? machineName = null, DateTime dateTime = default)
            : this(Guid.NewGuid(), logLevel, text, source, tag, machineName, dateTime)
        {
        }

        /// <summary>
        ///     Gets the unique identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public Guid Id { get; }

        /// <summary>
        ///     Gets the date time.
        /// </summary>
        /// <value>The date time.</value>
        public DateTime DateTime { get; }

        /// <summary>
        ///     Gets the log level.
        /// </summary>
        /// <value>The log level.</value>
        public LogLevel LogLevel { get; }

        /// <summary>
        ///     Gets the source.
        /// </summary>
        /// <value>The source.</value>
        public string Source { get; }

        /// <summary>
        ///     Gets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public string? Tag { get; }

        /// <summary>
        ///     Gets the machine name.
        /// </summary>
        /// <value>The machine name.</value>
        public string? MachineName { get; }

        /// <summary>
        ///     Gets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text { get; }

        /// <summary>
        ///     Gets the metadata.
        /// </summary>
        /// <value>The metadata.</value>
        public IDictionary<string, object> Metadata => new ReadOnlyDictionary<string, object>(_metadata);

        /// <summary>
        ///     Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        ///     A value that indicates the relative order of the objects being compared.
        /// </returns>
        public int CompareTo(LogEntry other)
        {
            return DateTime.CompareTo(other.DateTime);
        }
    }
}