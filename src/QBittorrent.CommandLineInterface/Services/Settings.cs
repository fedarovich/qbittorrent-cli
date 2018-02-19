using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace QBittorrent.CommandLineInterface.Services
{
    public class Settings
    {
        public const string DefaultUrl = "http://127.0.0.1:8080";

        [DefaultValue(DefaultUrl)]
        public string Url { get; set; } = DefaultUrl;

        public string Username { get; set; }

        public string Password { get; set; }
    }
}
