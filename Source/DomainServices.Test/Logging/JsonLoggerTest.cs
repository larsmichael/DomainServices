namespace DomainServices.Test.Logging
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using AutoFixture;
    using DomainServices.Logging;
    using Xunit;

    public class JsonLoggerTest : IDisposable
    {
        private readonly string _filePath = Path.Combine(Path.GetTempPath(), "__log.json");
        private readonly JsonLogger _logger;
        private readonly Fixture _fixture;

        public JsonLoggerTest()
        {
            _logger = new JsonLogger(_filePath);
            _fixture = new Fixture();
            _fixture.Register(() => LogLevel.Information);
        }

        public void Dispose()
        {
            File.Delete(_filePath);
        }

        [Fact]
        public void CreateWithNullFilePathThrows()
        {
            Assert.Throws<ArgumentNullException>(() => new JsonLogger(null!));
        }

        [Fact]
        public void LogAndGetIsOk()
        {
            var entry = _fixture.Create<LogEntry>();
            _logger.Log(entry);
            var actual = _logger.Get(entry.Id).Value;
            Assert.Equal(entry.Id, actual.Id);
        }

        [Fact]
        public void LogAndGetWithMetadataIsOk()
        {
            var id = Guid.NewGuid();
            var entry = new LogEntry(
                id,
                LogLevel.Error,
                "first error",
                "my-source",
                "my-tag",
                "my-machine",
                DateTime.Now,
                new Dictionary<string, object> { {"Description", "My log entry description"}}
            );
            _logger.Log(entry);

            var logEntry = _logger.Get(e => e.Id == id).Single();
            Assert.True(logEntry.Metadata.ContainsKey("Description"));
            Assert.Equal("My log entry description", logEntry.Metadata["Description"].ToString());
        }

        [Fact]
        public void EntryAddedEventIsRaisedOnLog()
        {
            var raisedEvents = new List<string>();
            _logger.EntryAdded += (_, _) => { raisedEvents.Add("Added"); };
            var entry = _fixture.Create<LogEntry>();
            _logger.Log(entry);
            Assert.Equal("Added", raisedEvents[0]);
        }

        [Fact]
        public void GetByQueryIsOk()
        {
            _logger.Log(new LogEntry(LogLevel.Error, "first error", "my-source", "my-tag", "my-machine", DateTime.Now.Subtract(TimeSpan.FromDays(3))));
            _logger.Log(new LogEntry(LogLevel.Error, "second error", "my-source", "my-tag", "my-machine", DateTime.Now.Subtract(TimeSpan.FromDays(2))));
            _logger.Log(new LogEntry(LogLevel.Information, "first info", "my-other-source", "my-tag", "my-machine", DateTime.Now.Subtract(TimeSpan.FromDays(2))));
            _logger.Log(new LogEntry(LogLevel.Error, "third error", "my-source", "my-tag", "my-machine", DateTime.Now.Subtract(TimeSpan.FromDays(1))));
            _logger.Log(new LogEntry(LogLevel.Information, "second info", "my-other-source", "my-tag", "my-machine", DateTime.Now.Subtract(TimeSpan.FromDays(1))));

            var queryErrors = new List<QueryCondition> { new QueryCondition("LogLevel", LogLevel.Error) };
            Assert.Equal(3, _logger.Get(queryErrors).ToArray().Length);

            var queryInfos = new List<QueryCondition> { new QueryCondition("LogLevel", LogLevel.Information) };
            Assert.Equal(2, _logger.Get(queryInfos).Count());

            var queryErrorsLastTwoDays = new List<QueryCondition>
            {
                new QueryCondition("LogLevel", QueryOperator.Equal, LogLevel.Error),
                new QueryCondition("DateTime", QueryOperator.GreaterThan, DateTime.Now.Subtract(TimeSpan.FromDays(2.1)))
            };
            Assert.Equal(2, _logger.Get(queryErrorsLastTwoDays).Count());
        }

        [Fact]
        public void GetByQueryUsingLogLevelComparisonIsOk()
        {
            _logger.Log(new LogEntry(LogLevel.Error, "first error", "my-source", "my-tag", "my-machine", DateTime.Now.Subtract(TimeSpan.FromDays(3))));
            _logger.Log(new LogEntry(LogLevel.Warning, "first warning", "my-source", "my-tag", "my-machine", DateTime.Now.Subtract(TimeSpan.FromDays(2))));
            _logger.Log(new LogEntry(LogLevel.Information, "first info", "my-other-source", "my-tag", "my-machine", DateTime.Now.Subtract(TimeSpan.FromDays(2))));
            _logger.Log(new LogEntry(LogLevel.Error, "second error", "my-source", "my-tag", "my-machine", DateTime.Now.Subtract(TimeSpan.FromDays(1))));
            _logger.Log(new LogEntry(LogLevel.Information, "second info", "my-other-source", "my-tag", "my-machine", DateTime.Now.Subtract(TimeSpan.FromDays(1))));

            var query = new List<QueryCondition> { new QueryCondition("LogLevel", QueryOperator.GreaterThan, LogLevel.Information) };
            Assert.Equal(3, _logger.Get(query).ToArray().Length);

            query = new List<QueryCondition> { new QueryCondition("LogLevel", QueryOperator.LessThanOrEqual, LogLevel.Warning) };
            Assert.Equal(3, _logger.Get(query).ToArray().Length);
        }

        [Fact]
        public void LastIsOk()
        {
            _logger.Log(new LogEntry(LogLevel.Error, "first error", "my-source", "my-tag", "my-machine", DateTime.Now.Subtract(TimeSpan.FromDays(3))));
            _logger.Log(new LogEntry(LogLevel.Error, "second error", "my-source", "my-tag", "my-machine", DateTime.Now.Subtract(TimeSpan.FromDays(2))));
            _logger.Log(new LogEntry(LogLevel.Information, "first info", "my-other-source", "my-tag", "my-machine", DateTime.Now.Subtract(TimeSpan.FromDays(2))));
            _logger.Log(new LogEntry(LogLevel.Error, "third error", "my-source", "my-tag", "my-machine", DateTime.Now.Subtract(TimeSpan.FromDays(1))));
            _logger.Log(new LogEntry(LogLevel.Information, "second info", "my-other-source", "my-tag", "my-machine", DateTime.Now.Subtract(TimeSpan.FromDays(1))));


            var queryErrors = new List<QueryCondition> { new QueryCondition("LogLevel", LogLevel.Error) };
            Assert.Equal("third error", _logger.Last(queryErrors).Value.Text);

            var queryInfos = new List<QueryCondition> { new QueryCondition("LogLevel", LogLevel.Information) };
            Assert.Equal("second info", _logger.Last(queryInfos).Value.Text);

            var queryErrorsOlderThanTwoDays = new List<QueryCondition>
            {
                new QueryCondition("LogLevel", QueryOperator.Equal, LogLevel.Error),
                new QueryCondition("DateTime", QueryOperator.LessThan, DateTime.Now.Subtract(TimeSpan.FromDays(2.1)))
            };
            Assert.Equal("first error", _logger.Last(queryErrorsOlderThanTwoDays).Value.Text);
        }

        [Fact]
        public void GetByQueryReturnsImmutables()
        {
            var entry = _fixture.Create<LogEntry>();
            _logger.Log(entry);
            Assert.Throws<NotSupportedException>(() => entry.Metadata.Add("Description", "MyDescription"));
        }
    }
}