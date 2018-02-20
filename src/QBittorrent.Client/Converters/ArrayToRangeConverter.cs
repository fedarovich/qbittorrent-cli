using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace QBittorrent.Client.Converters
{
    /// <summary>
    /// Converts JSON array of to elements to <see cref="Range"/>.
    /// </summary>
    /// <seealso cref="Newtonsoft.Json.JsonConverter" />
    public class ArrayToRangeConverter : JsonConverter
    {
        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var range = (Range) value;
            writer.WriteStartArray();
            writer.WriteValue(range.StartIndex);
            writer.WriteValue(range.EndIndex);
            writer.WriteEndArray();
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>
        /// The object value.
        /// </returns>
        /// <exception cref="JsonSerializationException">
        /// Not-null value was expected.
        /// or
        /// or
        /// Expected array of 2 elements.
        /// or
        /// </exception>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                if (objectType != typeof(Range?))
                    throw new JsonSerializationException("Not-null value was expected.");

                return null;
            }

            if (reader.TokenType == JsonToken.StartArray)
            {
                var list = new List<long>(2);
                while (reader.Read() && reader.TokenType != JsonToken.EndArray)
                {
                    if (reader.TokenType != JsonToken.Integer)
                        throw new JsonSerializationException($"Unexpected token {reader.TokenType}.");
                    list.Add(Convert.ToInt64(reader.Value));
                }

                if (list.Count != 2)
                    throw new JsonSerializationException("Expected array of 2 elements.");
                return new Range(list[0], list[1]);
            }

            throw new JsonSerializationException($"Unexpected token {reader.TokenType}.");
        }

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Range) || objectType == typeof(Range?);
        }
    }
}
