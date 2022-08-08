namespace DomainServices
{
    using System.Text.Json.Serialization;
    using System;

    /// <summary>
    ///     Class representing a date range.
    /// </summary>
    public class DateRange
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DateRange" /> class.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <exception cref="ArgumentException">Range start '{from}' must be less than, or equal to, range end '{to}'.</exception>
        [JsonConstructor]
        public DateRange(DateTime? from = null, DateTime? to = null)
        {
            var start = from ?? DateTime.MinValue;
            var end = to ?? DateTime.MaxValue;

            if (start > end)
            {
                throw new ArgumentException($"Range start '{start}' must be less than, or equal to, range end '{end}'.");
            }

            From = start;
            To = end;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DateRange" /> class.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="duration">The duration.</param>
        public DateRange(DateTime from, TimeSpan duration)
            : this(from, from + duration)
        {
        }

        /// <summary>
        ///     Gets the start datetime.
        /// </summary>
        public DateTime From { get; }

        /// <summary>
        ///     Gets the end datetime.
        /// </summary>
        public DateTime To { get; }

        /// <summary>
        ///     Checks whether the specified datetime is included in the date range.
        /// </summary>
        /// <param name="value">The datetime value.</param>
        /// <returns><c>true</c> if the given datetime is included in the date range, <c>false</c> otherwise.</returns>
        public bool Includes(DateTime value)
        {
            return From <= value && value <= To;
        }

        /// <summary>
        ///     Checks whether the specified datetime is strictly included in the date range.
        /// </summary>
        /// <param name="value">The datetime value.</param>
        /// <returns><c>true</c> if the given datetime is strictly included in the date range, <c>false</c> otherwise.</returns>
        public bool StrictlyIncludes(DateTime value)
        {
            return From < value && value < To;
        }

        /// <summary>
        ///     Gets the intersection with another date range.
        /// </summary>
        /// <param name="anotherRange">the other date range.</param>
        public Maybe<DateRange> GetIntersection(DateRange anotherRange)
        {
            if ((To < anotherRange.From) || (From > anotherRange.To))
            {
                return Maybe.Empty<DateRange>();
            }

            var from = From > anotherRange.From ? From : anotherRange.From;
            var to = To < anotherRange.To ? To : anotherRange.To;

            return new DateRange(from, to).ToMaybe();
        }

        /// <summary>
        ///     Converts to timespan.
        /// </summary>
        public TimeSpan ToTimeSpan()
        {
            return To - From;
        }

        public override string ToString()
        {
            return $"From: '{From}'; To: '{To}'.";
        }
    }
}