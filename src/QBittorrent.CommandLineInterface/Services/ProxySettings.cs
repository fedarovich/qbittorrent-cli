using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Newtonsoft.Json;
using QBittorrent.CommandLineInterface.Converters;

namespace QBittorrent.CommandLineInterface.Services
{
    public class ProxySettings
    {
        public Uri Address { get; set; }

        public bool BypassLocal { get; set; }

        public IList<string> Bypass { get; set; }

        public string Username { get; set; }

        [JsonConverter(typeof(EncryptConverter))]
        public string Password { get; set; }
    }
}
