using System;
using Newtonsoft.Json;

namespace QBittorrent.Client.Converters
{
    internal class SecondsToTimeSpanConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteValue(-1);
                return;
            }

            writer.WriteValue(value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            if (reader.TokenType == JsonToken.Integer)
            {
                var seconds = Convert.ToInt32(reader.Value);
                return new TimeSpan(0, 0, seconds);
            }

            throw new JsonSerializationException($"Unexpected token {reader.TokenType} when parsing integer.");
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(TimeSpan?);
        }
    }
}
