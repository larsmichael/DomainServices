namespace DomainServices.Test.Logging
{
    using System;
    using DomainServices.Logging;
    using Xunit;

    public class LogEntryTest
    {
        [Fact]
        public void CreateWithNullOrEmptySourceThrows()
        {
            Assert.Throws<ArgumentNullException>(() => new LogEntry(LogLevel.Error, "SomeError", null!));
            Assert.Throws<ArgumentException>(() => new LogEntry(LogLevel.Error, "SomeError", ""));
        }

        [Fact]
        public void CreateWithNullOrEmptyTextThrows()
        {
            Assert.Throws<ArgumentNullException>(() => new LogEntry(LogLevel.Error, null!, null!));
            Assert.Throws<ArgumentException>(() => new LogEntry(LogLevel.Error, "", null!));
        }

        [Fact]
        public void CreateGeneratesUniqueId()
        {
            var entry = new LogEntry(LogLevel.Information, "my-text", "my-source");
            Assert.IsType<Guid>(entry.Id);
        }

        [Fact]
        public void CreateSetsDateTimeNow()
        {
            var entry = new LogEntry(LogLevel.Information, "my-text", "my-source");
            Assert.True(DateTime.Now - entry.DateTime < TimeSpan.FromMinutes(1));
        }

        [Fact]
        public void CreateSetsMachineName()
        {
            var entry = new LogEntry(LogLevel.Information, "my-text", "my-source");
            Assert.Equal(Environment.GetEnvironmentVariable("COMPUTERNAME"), entry.MachineName);
        }

        [Fact]
        public void IsImmutable()
        {
            var entry = new LogEntry(LogLevel.Information, "my-text", "my-source");
            Assert.Throws<NotSupportedException>(() => entry.Metadata.Add("Description", "Entry description"));
        }
    }
}