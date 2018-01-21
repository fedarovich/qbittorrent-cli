using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace QBittorrent.Client.Converters
{
    internal class NegativeToNullConverter : JsonConverter
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

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            if (reader.TokenType == JsonToken.Integer)
            {
                if (objectType == typeof(int?))
                {
                    var @int = Convert.ToInt32(reader.Value);
                    return @int >= 0 ? @int : default(int?);
                }

                if (objectType == typeof(long?))
                {
                    var @long = Convert.ToInt64(reader.Value);
                    return @long >= 0L ? @long : default(long?);
                }
            }

            throw new JsonSerializationException($"Unexpected token {reader.TokenType} when parsing integer.");
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(int?) || objectType == typeof(long?);
        }
    }
}
