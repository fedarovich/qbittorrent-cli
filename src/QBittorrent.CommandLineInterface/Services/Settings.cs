using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QBittorrent.CommandLineInterface.Converters;

namespace QBittorrent.CommandLineInterface.Services
{
    public class Settings
    {
        public const string DefaultUrl = "http://127.0.0.1:8080";

        [DefaultValue(DefaultUrl)]
        public string Url { get; set; } = DefaultUrl;

        public string Username { get; set; }

        [JsonConverter(typeof(EncryptConverter))]
        public string Password { get; set; }

        [JsonExtensionData]
        public IDictionary<string, JToken> Other { get; set; }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (string.IsNullOrWhiteSpace(Url))
            {
                Url = DefaultUrl;
            }
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext context)
        {
            if (string.IsNullOrWhiteSpace(Url))
            {
                Url = DefaultUrl;
            }
        }
    }
}
