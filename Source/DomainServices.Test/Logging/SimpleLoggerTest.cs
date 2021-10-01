namespace DomainServices.Test.Logging
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using AutoFixture.Xunit2;
    using DomainServices.Logging;
    using Xunit;

    public class SimpleLoggerTest
    {
        [Theory]
        [AutoData]
        public void LogToFileIsOk(LogEntry logEntry)
        {
            var filePath = Path.Combine(Path.GetTempPath(), "__test.log");
            var logger = new SimpleLogger(filePath);

            logger.Log(logEntry);

            Assert.True(File.Exists(filePath));
            var lines = File.ReadLines(filePath);
            Assert.Single(lines);

            File.Delete(filePath);
        }

        [Theory]
        [AutoData]
        public void LogToFileInNonExistingFolderIsOk(LogEntry logEntry)
        {
            var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var filePath = Path.Combine(path, "__test.log");
            var logger = new SimpleLogger(filePath);

            logger.Log(logEntry);

            Assert.True(File.Exists(filePath));
            var lines = File.ReadLines(filePath);
            Assert.Single(lines);

            Directory.Delete(path, true);
        }

        [Theory]
        [AutoData]
        public void EntryAddedEventIsRaisedOnLog(LogEntry logEntry)
        {
            var filePath = Path.Combine(Path.GetTempPath(), "__test.log");
            var logger = new SimpleLogger(filePath);
            var raisedEvents = new List<string>();
            logger.EntryAdded += (_, _) => { raisedEvents.Add("Added"); };
            logger.Log(logEntry);
            Assert.Equal("Added", raisedEvents[0]);
            File.Delete(filePath);
        }

        [Fact]
        public void CreateWithNullOrEmptyFilePathThrows()
        {
            var e = Assert.Throws<ArgumentNullException>(() => new SimpleLogger(null!));
            Assert.Contains("Value cannot be null. (Parameter 'filePath')", e.Message);

            var e2 = Assert.Throws<ArgumentException>(() => new SimpleLogger(""));
            Assert.Contains("Required input filePath was empty. (Parameter 'filePath')", e2.Message);
        }
    }
}