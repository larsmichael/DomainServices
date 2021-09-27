namespace DomainServices.Logging
{
    using System.Collections.Generic;

    /// <summary>
    ///     Log reader abstraction
    /// </summary>
    public interface ILogReader
    {
        /// <summary>
        ///     Queries the log using a list of query conditions.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>IEnumerable&lt;LogEntry&gt;.</returns>
        IEnumerable<LogEntry> Get(IEnumerable<QueryCondition> query);

        /// <summary>
        ///     Queries the last log entry using a list of query conditions.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>IEnumerable&lt;LogEntry&gt;.</returns>
        Maybe<LogEntry> Last(IEnumerable<QueryCondition> query);
    }
}