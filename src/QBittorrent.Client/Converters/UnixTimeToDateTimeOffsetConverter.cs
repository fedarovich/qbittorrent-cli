using System;
using Newtonsoft.Json;

namespace QBittorrent.Client.Converters
{
    public class UnixTimeToDateTimeOffsetConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                var dateTimeOffset = (DateTimeOffset) value;
                writer.WriteValue(dateTimeOffset.ToUnixTimeSeconds());
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            bool isNullable = objectType == typeof(DateTimeOffset?);
            if (reader.TokenType == JsonToken.Null)
            {
                return isNullable ? (DateTimeOffset?)null : throw new JsonSerializationException($"Cannot convert null value to {objectType}.");
            }
            if (reader.TokenType == JsonToken.Integer)
            {
                var unixTime = Convert.ToInt64(reader.Value);
                if (unixTime < 0)
                    return isNullable ? (DateTimeOffset?)null : throw new JsonSerializationException($"Cannot convert null value to {objectType}.");
                return DateTimeOffset.FromUnixTimeSeconds(unixTime);
            }

            throw new JsonSerializationException($"Unexpected token {reader.TokenType} when parsing unix time.");
        }

        public override bool CanConvert(Type objectType) => objectType == typeof(DateTimeOffset) || objectType == typeof(DateTimeOffset?);
    }
}
