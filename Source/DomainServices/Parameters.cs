namespace DomainServices
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using Ardalis.GuardClauses;

    /// <summary>
    ///     Class Parameters.
    /// </summary>
    [Serializable]
    public class Parameters : Dictionary<string, string>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Parameters" /> class.
        /// </summary>
        public Parameters()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Parameters" /> class.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        public Parameters(IDictionary<string, string> parameters) : base(parameters)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Parameters"/> class.
        /// </summary>
        /// <param name="information">The serialization information.</param>
        /// <param name="context">The context.</param>
        protected Parameters(SerializationInfo information, StreamingContext context) : base(information, context)
        {
        }

        /// <summary>
        ///     Parses the specified parameters string and converts it into a Parameters object.
        /// </summary>
        /// <param name="s">The parameters string.</param>
        /// <returns>Parameters.</returns>
        public static Parameters Parse(string s)
        {
            Guard.Against.NullOrEmpty(s, nameof(s));
            Dictionary<string, string> dictionary;
            try
            {
                dictionary = s.Split(';').ToDictionary(r => r.Split('=')[0], r => r.Split('=')[1]);
            }
            catch
            {
                var sb = new StringBuilder();
                sb.AppendLine($"Could not parse parameters string '{s}'.");
                sb.AppendLine("A parameters string has the format \"param1=value1;param2=value2;...\".");
                sb.AppendLine("For example: \"Item=WaterLevel;X=123.4;Y=4.567\".");
                throw new ArgumentException(sb.ToString(), nameof(s));
            }
            
            return new Parameters(dictionary);
        }

        /// <summary>
        ///     Gets the parameter with the specified key.
        ///     If the key is not found, the default value is returned.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>System.Int32.</returns>
        public int GetParameter(string key, int defaultValue)
        {
            var result = defaultValue;
            if (ContainsKey(key))
            {
                result = int.Parse(this[key], CultureInfo.InvariantCulture);
            }

            return result;
        }

        /// <summary>
        ///     Gets the parameter with the specified key.
        ///     If the key is not found, the default value is returned.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>System.Double.</returns>
        public double GetParameter(string key, double defaultValue)
        {
            var result = defaultValue;
            if (ContainsKey(key))
            {
                result = double.Parse(this[key], CultureInfo.InvariantCulture);
            }

            return result;
        }

        /// <summary>
        ///     Gets the parameter with the specified key.
        ///     If the key is not found, the default value is returned.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>DateTime.</returns>
        public DateTime GetParameter(string key, DateTime defaultValue)
        {
            var result = defaultValue;
            if (ContainsKey(key))
            {
                result = DateTime.Parse(this[key], CultureInfo.InvariantCulture);
            }

            return result;
        }

        /// <summary>
        ///     Gets the parameter with the specified key.
        ///     If the key is not found, the default value is returned.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>TimeSpan.</returns>
        public TimeSpan GetParameter(string key, TimeSpan defaultValue)
        {
            var result = defaultValue;
            if (ContainsKey(key))
            {
                result = TimeSpan.Parse(this[key], CultureInfo.InvariantCulture);
            }

            return result;
        }

        /// <summary>
        ///     Gets the parameter with the specified key.
        ///     If the key is not found, the default value is returned.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">he default value.</param>
        /// <returns><c>true</c> if parameter is true, <c>false</c> otherwise.</returns>
        public bool GetParameter(string key, bool defaultValue)
        {
            var result = defaultValue;
            if (ContainsKey(key))
            {
                result = bool.Parse(this[key]);
            }

            return result;
        }

        /// <summary>
        ///     Gets the parameter with the specified key.
        ///     If the key is not found, the default value is returned.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>System.String.</returns>
        public Guid GetParameter(string key, Guid defaultValue)
        {
            var result = defaultValue;
            if (ContainsKey(key))
            {
                result = new Guid(this[key]);
            }

            return result;
        }

        /// <summary>
        ///     Gets the parameter with the specified key.
        ///     If the key is not found, the default value is returned.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>System.String.</returns>
        public string GetParameter(string key, string defaultValue)
        {
            var result = defaultValue;
            if (ContainsKey(key))
            {
                result = this[key];
            }

            return result;
        }

        /// <summary>
        ///     Try to get the parameter with the specified key.
        ///     If the key is not found, the default value of the specified type is returned.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if a parameter with specified key and of the specified type is found, <c>false</c> otherwise.</returns>
        public bool TryGetParameter<T>(string key, out T value)
        {
            try
            {
                var o = GetParameter(key);
                if (o is T val)
                {
                    value = val;
                    return true;
                }

                value = default;
                return false;

            }
            catch 
            {
                value = default;
                return false;
            }
        }

        /// <summary>
        ///     Gets the parameter with the specified key.
        ///     If the key is not found, an exception is thrown.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>System.Object.</returns>
        /// <exception cref="KeyNotFoundException">Parameter '{key}' was not found.</exception>
        public object GetParameter(string key)
        {
            if (!ContainsKey(key))
            {
                throw new KeyNotFoundException($"Parameter '{key}' was not found.");
            }

            if (int.TryParse(this[key], NumberStyles.Integer, CultureInfo.InvariantCulture, out var intValue))
            {
                return intValue;
            }

            if (double.TryParse(this[key], NumberStyles.Number, CultureInfo.InvariantCulture, out var doubleValue))
            {
                return doubleValue;
            }

            if (Guid.TryParse(this[key], out var guidValue))
            {
                return guidValue;
            }

            if (TimeSpan.TryParse(this[key], CultureInfo.InvariantCulture, out var timeSpanValue))
            {
                return timeSpanValue;
            }

            if (DateTime.TryParse(this[key], CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTimeValue))
            {
                return dateTimeValue;
            }

            if (bool.TryParse(this[key], out var boolValue))
            {
                return boolValue;
            }

            return this[key];
        }

        /// <summary>
        ///     Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public override string ToString()
        {
            var builder = new StringBuilder();
            if (!this.Any())
            {
                return base.ToString();
            }

            var sortedKeys = Keys.ToList();
            sortedKeys.Sort();
            var connector = "";
            foreach (var key in sortedKeys)
            {
                builder.Append(connector + key + "=" + this[key]);
                connector = ";";
            }

            return builder.ToString();

        }
    }
}