namespace DomainServices.Test
{
    using System;
    using AutoFixture.Xunit2;
    using Xunit;

    public class DateRangeTest
    {
        [Fact]
        public void CreateWithIllegalIntervalThrows()
        {
            var e = Assert.Throws<ArgumentException>(() => new DateRange(DateTime.Now, DateTime.Now.AddHours(-1)));
            Assert.Contains("must be less than, or equal to, range end", e.Message);
        }

        [Fact]
        public void CreateWithIllegalTimeSpanThrows()
        {
            var e = Assert.Throws<ArgumentException>(() => new DateRange(DateTime.Now, TimeSpan.FromHours(-1)));
            Assert.Contains("must be less than, or equal to, range end", e.Message);
        }

        [Theory, AutoData]
        public void CreateWithTimeSpanIsOk(TimeSpan duration)
        {
            var range = new DateRange(DateTime.Now, duration);
            Assert.Equal(duration, range.ToTimeSpan());
        }

        [Fact]
        public void CreateWithZeroTimeSpanIsOk()
        {
            var range = new DateRange(DateTime.Now, TimeSpan.Zero);
            Assert.Equal(TimeSpan.Zero, range.ToTimeSpan());
            Assert.Equal(range.From, range.To);
        }

        [Fact]
        public void CreateWhereFromEqualsToIsOk()
        {
            var dateTime = DateTime.Now;
            var range = new DateRange(dateTime, dateTime);
            Assert.Equal(TimeSpan.Zero, range.ToTimeSpan());
            Assert.Equal(range.From, range.To);
        }

        [Fact]
        public void CreateWhithoutToIsOk()
        {
            var dateTime = DateTime.Now;
            var range = new DateRange(dateTime);
            Assert.Equal(dateTime, range.From);
            Assert.Equal(DateTime.MaxValue, range.To);
        }

        [Fact]
        public void CreateWithoutFromIsOk()
        {
            var dateTime = DateTime.Now;
            var range = new DateRange(to: dateTime);
            Assert.Equal(DateTime.MinValue, range.From);
            Assert.Equal(dateTime, range.To);
        }

        [Fact]
        public void CreateWithoutFromAndToIsOk()
        {
            var range = new DateRange();
            Assert.Equal(DateTime.MinValue, range.From);
            Assert.Equal(DateTime.MaxValue, range.To);
        }

        [Fact]
        public void IncludesIsOk()
        {
            var now = DateTime.Now;
            var range = new DateRange(now, now.AddHours(1));
            Assert.True(range.Includes(now.AddMinutes(30)));
            Assert.True(range.Includes(now.AddHours(1)));
            Assert.False(range.Includes(now.AddHours(2)));
        }

        [Fact]
        public void IncludesForZeroTimeSpanIsOk()
        {
            var dateTime = DateTime.Now;
            var range = new DateRange(dateTime, dateTime);
            Assert.True(range.Includes(dateTime));
            Assert.False(range.Includes(dateTime.AddHours(1)));
        }

        [Fact]
        public void StrictlyIncludesIsOk()
        {
            var now = DateTime.Now;
            var range = new DateRange(now, now.AddHours(1));
            Assert.True(range.StrictlyIncludes(now.AddMinutes(30)));
            Assert.False(range.StrictlyIncludes(now.AddHours(1)));
            Assert.False(range.StrictlyIncludes(now.AddHours(2)));
        }

        [Fact]
        public void StrictlyIncludesForZeroTimeSpanIsOk()
        {
            var dateTime = DateTime.Now;
            var range = new DateRange(dateTime, dateTime);
            Assert.False(range.StrictlyIncludes(dateTime));
            Assert.False(range.StrictlyIncludes(dateTime.AddHours(1)));
        }

        [Fact]
        public void ToTimeSpanIsOk()
        {
            var now = DateTime.Now;
            var range = new DateRange(now, now.AddHours(1));
            Assert.Equal(TimeSpan.FromHours(1), range.ToTimeSpan());
        }

        [Fact]
        public void GetIntersectionReturnsEmptyMaybeIfNoIntersection()
        {
            // --- range ---|
            //                 |--- anotherRange ---
            var now = DateTime.Now;
            var range = new DateRange(to: now);
            var anotherRange = new DateRange(from: now.AddHours(1));
            Assert.False(range.GetIntersection(anotherRange).HasValue);

            //                        |--- range ---
            // --- anotherRange ---|
            range = new DateRange(from: now.AddHours(1));
            anotherRange = new DateRange(to: now);
            Assert.False(range.GetIntersection(anotherRange).HasValue);
        }

        [Fact]
        public void GetIntersectionIsOk()
        {
            // --- range ---|
            //           |--- anotherRange ---|
            var now = DateTime.Now;
            var range = new DateRange(to: now);
            var anotherRange = new DateRange(from: now.AddHours(-1));
            Assert.True(range.GetIntersection(anotherRange).HasValue);
            var intersection = range.GetIntersection(anotherRange).Value;
            Assert.Equal(now.AddHours(-1), intersection.From);
            Assert.Equal(now, intersection.To);


            //                  |--- range ---
            // --- anotherRange ---|
            range = new DateRange(from: now);
            anotherRange = new DateRange(to: now.AddHours(1));
            Assert.True(range.GetIntersection(anotherRange).HasValue);
            intersection = range.GetIntersection(anotherRange).Value;
            Assert.Equal(now, intersection.From);
            Assert.Equal(now.AddHours(1), intersection.To);

            // --------- range ---------
            //  |--- anotherRange ---| 
            range = new DateRange();
            anotherRange = new DateRange(now, TimeSpan.FromHours(1));
            Assert.True(range.GetIntersection(anotherRange).HasValue);
            intersection = range.GetIntersection(anotherRange).Value;
            Assert.Equal(now, intersection.From);
            Assert.Equal(now.AddHours(1), intersection.To);

            //   |--- range ---|
            // ---anotherRange ---
            range = new DateRange(now, TimeSpan.FromHours(1));
            anotherRange = new DateRange();
            Assert.True(range.GetIntersection(anotherRange).HasValue);
            intersection = range.GetIntersection(anotherRange).Value;
            Assert.Equal(now, intersection.From);
            Assert.Equal(now.AddHours(1), intersection.To);
        }

        [Fact]
        public void ToStringIsOk()
        {
            var now = DateTime.Now;
            var range = new DateRange(now, now.AddHours(1));
            Assert.Equal($"From: '{now}'; To: '{now.AddHours(1)}'.", range.ToString());
        }
    }
}