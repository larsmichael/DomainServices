﻿using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DomainServices.Test")]
namespace DomainServices
{
    using System;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    /// <summary>
    ///     Deserialize inferred types to object properties
    /// </summary>
    /// <remarks>
    ///     https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-converters-how-to?pivots=dotnet-6-0#deserialize-inferred-types-to-object-properties
    /// </remarks>
    internal class ObjectToInferredTypesConverter : JsonConverter<object>
    {
        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.TokenType switch
            {
                JsonTokenType.True => true,
                JsonTokenType.False => false,
                JsonTokenType.Number when reader.TryGetInt64(out long l) => l,
                JsonTokenType.Number => reader.GetDouble(),
                JsonTokenType.String when reader.TryGetDateTime(out DateTime datetime) => datetime,
                JsonTokenType.String => reader.GetString()!,
                _ => JsonDocument.ParseValue(ref reader).RootElement.Clone()
            };
        }

        public override void Write(Utf8JsonWriter writer, object objectToWrite, JsonSerializerOptions options)
        {
            if (objectToWrite.GetType() == typeof(object))
            {
                writer.WriteStartObject();
                writer.WriteEndObject();
            }
            else
            {
                JsonSerializer.Serialize(writer, objectToWrite, objectToWrite.GetType(), options);
            }
        }
    }
}