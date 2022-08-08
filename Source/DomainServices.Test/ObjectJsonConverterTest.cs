namespace DomainServices.Test
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json;
    using Xunit;

    public class ObjectJsonConverterTest
    {
        private readonly JsonSerializerOptions _options;
        public ObjectJsonConverterTest()
        {
            _options = new JsonSerializerOptions();
            _options.Converters.Add(new ObjectJsonConverter());
        }

        [Fact]
        // This is the problem the ObjectJsonConverter is solving
        public void ByDefaultObjectPropertiesAreDeserializedToJsonElement()
        {
            var dictionary = new Dictionary<string, object>
            {
                {"foo", true},
                {"bar", DateTime.Now},
                {"baz", 123.456}
            };

            var json = JsonSerializer.Serialize(dictionary);
            var deserialized = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            Assert.NotNull(deserialized);
            foreach (var value in deserialized.Values)
            {
                Assert.IsType<JsonElement>(value);
            }
        }

        [Fact]
        public void ThrowsWhenWritingObjectProperty()
        {
            var dictionary = new Dictionary<string, object>
            {
                {"foo", true},
                {"bar", DateTime.Now},
                {"baz", 123.456}
            };

            var e = Assert.Throws<InvalidOperationException>(() => JsonSerializer.Serialize(dictionary, _options));
            Assert.Equal($"The {nameof(ObjectJsonConverter)} does not support writing.", e.Message);
        }

        [Fact]
        public void DateTimeIsOk()
        {
            var dateTime = DateTime.Now;
            var json = JsonSerializer.Serialize(dateTime);
            var result = JsonSerializer.Deserialize<object>(json, _options);
            Assert.IsType<DateTime>(result);
            Assert.Equal(dateTime, result);
        }

        [Fact]
        public void IntIsOk()
        {
            const int number = 999;
            var json = JsonSerializer.Serialize(number);
            var result = JsonSerializer.Deserialize<object>(json, _options);
            Assert.IsType<long>(result);
            Assert.Equal((long)number, result);
        }

        [Fact]
        public void LongIsOk()
        {
            const long number = 999;
            var json = JsonSerializer.Serialize(number);
            var result = JsonSerializer.Deserialize<object>(json, _options);
            Assert.IsType<long>(result);
            Assert.Equal(number, result);
        }

        [Fact]
        public void DoubleIsOk()
        {
            const double number = 999.99;
            var json = JsonSerializer.Serialize(number);
            var result = JsonSerializer.Deserialize<object>(json, _options);
            Assert.IsType<double>(result);
            Assert.Equal(number, result);
        }

        [Fact]
        public void FloatIsOk()
        {
            const float number = 999.99f;
            var json = JsonSerializer.Serialize(number);
            var result = JsonSerializer.Deserialize<object>(json, _options);
            Assert.IsType<double>(result);
            Assert.Equal(number, (double)result, 2);
        }

        [Fact]
        public void BoolIsOk()
        {
            const bool b = true;
            var json = JsonSerializer.Serialize(b);
            var result = JsonSerializer.Deserialize<object>(json, _options);
            Assert.IsType<bool>(result);
            Assert.Equal(b, result);
        }
    }
}
