﻿using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DomainServices.Test")]
namespace DomainServices
{
    using System;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    internal class ObjectJsonConverter : JsonConverter<object>
    {
        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.True)
            {
                return true;
            }

            if (reader.TokenType == JsonTokenType.False)
            {
                return false;
            }

            if (reader.TokenType == JsonTokenType.Number)
            {
                if (reader.TryGetInt64(out long l))
                {
                    return l;
                }

                return reader.GetDouble();
            }

            if (reader.TokenType == JsonTokenType.String)
            {
                if (reader.TryGetDateTime(out DateTime datetime))
                {
                    return datetime;
                }

                return reader.GetString()!;
            }

            using JsonDocument document = JsonDocument.ParseValue(ref reader);
            return document.RootElement.Clone();
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            throw new InvalidOperationException($"The {nameof(ObjectJsonConverter)} does not support writing.");
        }
    }
}
