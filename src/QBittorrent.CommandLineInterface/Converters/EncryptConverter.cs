using System;
using Newtonsoft.Json;
using QBittorrent.CommandLineInterface.Services;

namespace QBittorrent.CommandLineInterface.Converters
{
    public class EncryptConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var str = (string)value;
            writer.WriteValue(EncryptionService.Instance.Encrypt(str));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            if (reader.TokenType == JsonToken.String)
            {
                return EncryptionService.Instance.Decrypt((string) reader.Value);
            }

            throw new JsonSerializationException($"Unexpected token {reader.TokenType}.");
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }
    }
}
