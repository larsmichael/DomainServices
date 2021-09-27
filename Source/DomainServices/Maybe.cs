#pragma warning disable CA1815 // Override equals and operator equals on value types

namespace DomainServices
{
    using System;

    /// <summary>
    ///     Struct Maybe. A Maybe&lt;T&gt; is an object which can have a value or not.
    /// </summary>
    /// <typeparam name="T">Type of value.</typeparam>
    public readonly struct Maybe<T>

    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Maybe{T}" /> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentNullException">value</exception>
        public Maybe(T value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            Value = value;
            HasValue = true;
        }

        /// <summary>
        ///     Gets a value indicating whether this instance has a value.
        /// </summary>
        /// <value><c>true</c> if this instance has a value; otherwise, <c>false</c>.</value>
        public bool HasValue { get; }

        /// <summary>
        ///     Gets the value of this instance
        /// </summary>
        public T Value { get; }

        /// <summary>
        ///     Gets value of this instance (<paramref name="c1" />), if it has a value, otherwise it returns
        ///     <paramref name="c2" />
        /// </summary>
        public static T operator |(Maybe<T> c1, T c2)
        {
            return c1.HasValue ? c1.Value : c2;
        }

        /// <summary>
        ///     Gets value of this instance (<paramref name="c1" />), if it has a value, otherwise it returns
        ///     <paramref name="c2" />
        /// </summary>
        public static T operator |(Maybe<T> c1, Func<T> c2)
        {
            return c1.HasValue ? c1.Value : c2();
        }
    }

    /// <summary>
    ///     Class Maybe.
    /// </summary>
    public static class Maybe
    {
        /// <summary>
        ///     Converts a value to a maybe of this value.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>Maybe&lt;T&gt;.</returns>
        public static Maybe<T> ToMaybe<T>(this T value)
        {
            return new Maybe<T>(value);
        }

        /// <summary>
        ///     Returns an empty maybe.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>Maybe&lt;T&gt;.</returns>
        public static Maybe<T> Empty<T>()
        {
            return new();
        }

        /// <summary>
        ///     Return value of this instance - for build compatibility
        /// </summary>
        [Obsolete("Use Value instead. This method will be removed in a future version.")]
        public static T Single<T>(this Maybe<T> maybe)
        {
            return maybe.Value;
        }
    }
}

#pragma warning restore CA1815 // Override equals and operator equals on value types