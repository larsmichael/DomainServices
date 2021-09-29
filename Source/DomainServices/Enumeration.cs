namespace DomainServices
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Abstract base class for enumerations
    /// </summary>
    public abstract class Enumeration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Enumeration"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="displayName">The display name.</param>
        protected Enumeration(int value, string displayName)
        {
            Value = value;
            DisplayName = displayName;
        }

        /// <summary>
        /// Gets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName { get; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public int Value { get; }

        /// <summary>
        /// Gets an enumeration from its display name.
        /// </summary>
        /// <typeparam name="TEnumeration">The type of the enumeration.</typeparam>
        /// <param name="displayName">The display name.</param>
        /// <returns>TEnumeration.</returns>
        public static TEnumeration FromDisplayName<TEnumeration>(string displayName) where TEnumeration : Enumeration
        {
            var matchingItem = Parse<TEnumeration, string>(displayName, "display name", item => item.DisplayName == displayName);
            return matchingItem;
        }

        /// <summary>
        /// Gets an enumeration from its value.
        /// </summary>
        /// <typeparam name="TEnumeration">The type of the enumeration.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>TEnumeration.</returns>
        public static TEnumeration FromValue<TEnumeration>(int value) where TEnumeration : Enumeration
        {
            var matchingItem = Parse<TEnumeration, int>(value, "value", item => item.Value == value);
            return matchingItem;
        }

        /// <summary>
        /// Gets all enumerations of the specified type.
        /// </summary>
        /// <typeparam name="TEnumeration">The type of the enumeration.</typeparam>
        /// <returns>IEnumerable&lt;TEnumeration&gt;.</returns>
        public static IEnumerable<TEnumeration> GetAll<TEnumeration>() where TEnumeration : Enumeration
        {
            var fields = typeof(TEnumeration).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
            return fields.Select(f => f.GetValue(null)).Cast<TEnumeration>();
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object? obj)
        {
            if (obj is not Enumeration otherValue)
            {
                return false;
            }

            var typeMatches = GetType().IsInstanceOfType(obj) || obj.GetType().IsAssignableFrom(GetType());
            var valueMatches = Value.Equals(otherValue.Value);

            return typeMatches && valueMatches;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return DisplayName;
        }

        private static TEnumeration Parse<TEnumeration, TValue>(TValue value, string description, Func<TEnumeration, bool> predicate) where TEnumeration : Enumeration
        {
            var matchingItem = GetAll<TEnumeration>().FirstOrDefault(predicate);

            if (matchingItem == null)
            {
                var message = $"'{value}' is not a valid {description} in {typeof(TEnumeration)}.";
                throw new ApplicationException(message);
            }

            return matchingItem;
        }
    }
}